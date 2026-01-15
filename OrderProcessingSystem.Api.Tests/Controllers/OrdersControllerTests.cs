using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderProcessingSystem.Api.Controllers;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById;
using OrderProcessingSystem.Data.Helper.Enums;
using Xunit;

namespace OrderProcessingSystem.Api.Tests.Controllers
{
    public class OrdersControllerTests
    {
        private readonly Mock<ISender> _senderMock;
        private readonly Mock<IValidator<CreateOrderRequest>> _validatorMock;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _senderMock = new Mock<ISender>(MockBehavior.Strict);
            _validatorMock = new Mock<IValidator<CreateOrderRequest>>(MockBehavior.Strict);

            _controller = new OrdersController(_senderMock.Object);
        }

        #region GET ALL ORDERS

        [Fact]
        public async Task Get_ShouldReturnOkResult()
        {
            // Arrange
            var resultData = new List<OrderDetailsDto>();

            _senderMock
                .Setup(x => x.Send(
                    It.IsAny<GetAllOrdersRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(resultData);

            // Act
            var result = await _controller.Get(OrderStatus.PENDING, Guid.NewGuid());

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(resultData, okResult.Value);

            _senderMock.VerifyAll();
        }

        #endregion

        #region GET ORDER BY ID

        [Fact]
        public async Task GetById_ShouldReturnOkResult()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var orderDto = new OrderDetailsDto();

            _senderMock
                .Setup(x => x.Send(
                    It.IsAny<GetOrderByIdRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderDto);

            // Act
            var result = await _controller.GetById(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderDto, okResult.Value);

            _senderMock.VerifyAll();
        }

        #endregion

        #region CREATE ORDER

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var request = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrdersOrderItem>());

            var validationErrors = new List<ValidationFailure>
            {
                new("CustomerId", "CustomerId is required")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationErrors));

            // Act
            var result = await _controller.Post(request, _validatorMock.Object);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(validationErrors, badRequest.Value);

            _validatorMock.VerifyAll();
        }

        [Fact]
        public async Task Post_ShouldReturnCreatedAtAction_WhenValidationSucceeds()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var request = new CreateOrderRequest(Guid.NewGuid(), new List<CreateOrdersOrderItem>());

            _validatorMock
                .Setup(v => v.ValidateAsync(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _senderMock
                .Setup(x => x.Send(
                    It.IsAny<CreateOrderRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderId);

            // Act
            var result = await _controller.Post(request, _validatorMock.Object);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(OrdersController.GetById), created.ActionName);
            Assert.Equal(orderId, created.RouteValues["orderId"]);

            _validatorMock.VerifyAll();
            _senderMock.VerifyAll();
        }

        #endregion

        #region CANCEL ORDER

        [Fact]
        public async Task CancelOrder_ShouldReturnNoContent()
        {
            // Arrange
            var orderId = Guid.NewGuid();

            _senderMock
                .Setup(x => x.Send(
                    It.IsAny<UpdateOrderStatusRequest>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Unit.Value));

            // Act
            var result = await _controller.CancelOrder(orderId);

            // Assert
            Assert.IsType<NoContentResult>(result);

            _senderMock.VerifyAll();
        }

        #endregion
    }
}
