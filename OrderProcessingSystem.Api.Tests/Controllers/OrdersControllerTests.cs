using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using OrderProcessingSystem.Api.Controllers;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Api.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private static async IAsyncEnumerable<OrderDetailsDto> ToAsyncEnumerable(params OrderDetailsDto[] items)
        {
            foreach (var it in items)
            {
                yield return it;
                await Task.Yield();
            }
        }

        [Fact]
        public async Task Get_ReturnsOk_WithOrdersAsyncEnumerable()
        {
            var mockSender = new Mock<ISender>();
            var dto1 = new OrderDetailsDto { OrderId = Guid.NewGuid(), CustomerName = "C1", TotalAmount = 10m };
            var dto2 = new OrderDetailsDto { OrderId = Guid.NewGuid(), CustomerName = "C2", TotalAmount = 20m };

            mockSender
                .Setup(s => s.Send(It.IsAny<GetAllOrdersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ToAsyncEnumerable(dto1, dto2));

            var controller = new OrdersController(mockSender.Object);

            var result = await controller.Get(null, null);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<IAsyncEnumerable<OrderDetailsDto>>(ok.Value!);

            var list = new List<OrderDetailsDto>();
            await foreach (var item in returned) list.Add(item);

            Assert.Equal(2, list.Count);
            Assert.Contains(list, x => x.OrderId == dto1.OrderId);
            Assert.Contains(list, x => x.OrderId == dto2.OrderId);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WithOrderDetails()
        {
            var orderId = Guid.NewGuid();
            var expected = new OrderDetailsDto { OrderId = orderId, CustomerName = "Alice", TotalAmount = 42m };

            var mockSender = new Mock<ISender>();
            mockSender
                .Setup(s => s.Send(It.Is<GetOrderByIdRequest>(r => r.OrderId == orderId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var controller = new OrdersController(mockSender.Object);

            var result = await controller.GetById(orderId);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(expected, ok.Value);
        }

        [Fact]
        public async Task Post_InvalidModel_ReturnsBadRequestWithErrors()
        {
            var invalid = new ValidationResult(new[] { new ValidationFailure("Items", "required") });
            var mockValidator = new Mock<IValidator<CreateOrderRequest>>();
            mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalid);

            var mockSender = new Mock<ISender>();
            var controller = new OrdersController(mockSender.Object);

            var request = new CreateOrderRequest(null, new List<CreateOrdersOrderItem>());

            var result = await controller.Post(request, mockValidator.Object);

            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(invalid.Errors, bad.Value);
        }

        [Fact]
        public async Task Post_ValidModel_ReturnsCreatedAtAction()
        {
            var newOrderId = Guid.NewGuid();
            var mockValidator = new Mock<IValidator<CreateOrderRequest>>();
            mockValidator
                .Setup(v => v.ValidateAsync(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult()); // valid

            var mockSender = new Mock<ISender>();
            mockSender
                .Setup(s => s.Send(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(newOrderId);

            var controller = new OrdersController(mockSender.Object);

            var req = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrdersOrderItem> { new CreateOrdersOrderItem(Guid.NewGuid(), 1) });

            var result = await controller.Post(req, mockValidator.Object);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(OrdersController.GetById), created.ActionName);

            Assert.True(created.RouteValues!.ContainsKey("orderId"));
            Assert.Equal(newOrderId, created.RouteValues["orderId"]);
            Assert.Null(created.Value);
        }

        [Fact]
        public async Task CancelOrder_SendsUpdateRequest_AndReturnsNoContent()
        {
            var orderId = Guid.NewGuid();
            var mockSender = new Mock<ISender>();

            // non-generic Send(IRequest) returns Task
            mockSender
                .Setup(s => s.Send(It.IsAny<IRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var controller = new OrdersController(mockSender.Object);

            var result = await controller.CancelOrder(orderId);

            Assert.IsType<NoContentResult>(result);

            mockSender.Verify(s => s.Send(It.Is<UpdateOrderStatusRequest>(r => r.Id == orderId && r.NewStatus == OrderStatus.CANCELED), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
