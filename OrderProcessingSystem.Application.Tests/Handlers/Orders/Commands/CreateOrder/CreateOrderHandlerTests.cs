using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Tests.Handlers.Orders.Commands.CreateOrder
{
    public class CreateOrderHandlerTests
    {
        private static OrderProcessingSystemContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new OrderProcessingSystemContext(options);
        }

        [Fact]
        public async Task Handle_ValidRequest_CallsRuleAndReturnsId()
        {
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);

            // seed customer and product that validation expects
            var customerId = Guid.NewGuid();
            context.Customers.Add(new Customer
            {
                Id = customerId,
                Name = "C",
                Email = "c@x",
                Phone = "1",
                PermanentAddress = "a",
                ShippingAddress = "a"
            });

            var productId = Guid.NewGuid();
            context.Products.Add(new Product
            {
                Id = productId,
                Name = "P",
                Description = "d",
                Price = 9.99m
            });

            await context.SaveChangesAsync();

            var expectedId = Guid.NewGuid();
            var mockRule = new Mock<ICreateOrderRule<CreateOrderRequest, Guid>>();
            CreateOrderRequest? capturedRequest = null;

            mockRule
                .Setup(r => r.Apply(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()))
                .Callback<CreateOrderRequest, CancellationToken>((req, ct) => capturedRequest = req)
                .ReturnsAsync(expectedId);

            var handler = new CreateOrderHandler(mockRule.Object, context);

            var request = new CreateOrderRequest(
                CustomerId: customerId,
                Items: new[] { new CreateOrdersOrderItem(ItemId: productId, Quantity: 2) }.ToList()
            );

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.Equal(expectedId, result);
            Assert.NotNull(capturedRequest);
            Assert.Equal(request.CustomerId, capturedRequest!.CustomerId);
            Assert.Single(capturedRequest.Items);
            Assert.Equal(productId, capturedRequest.Items.First().ItemId);

            mockRule.Verify(r => r.Apply(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCustomer_ThrowsBadRequestException()
        {
            using var context = CreateInMemoryContext(Guid.NewGuid().ToString());

            // seed a product but NOT the customer
            var productId = Guid.NewGuid();
            context.Products.Add(new Product
            {
                Id = productId,
                Name = "P",
                Description = "d",
                Price = 5m
            });
            await context.SaveChangesAsync();

            var mockRule = new Mock<ICreateOrderRule<CreateOrderRequest, Guid>>();
            var handler = new CreateOrderHandler(mockRule.Object, context);

            var request = new CreateOrderRequest(
                CustomerId: Guid.NewGuid(),
                Items: new[] { new CreateOrdersOrderItem(ItemId: productId, Quantity: 1) }.ToList()
            );

            await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(request, CancellationToken.None));
            mockRule.Verify(r => r.Apply(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_InvalidProductId_ThrowsBadRequestException()
        {
            using var context = CreateInMemoryContext(Guid.NewGuid().ToString());

            // seed customer but NOT the referenced product
            var customerId = Guid.NewGuid();
            context.Customers.Add(new Customer
            {
                Id = customerId,
                Name = "C",
                Email = "c@x",
                Phone = "1",
                PermanentAddress = "a",
                ShippingAddress = "a"
            });
            await context.SaveChangesAsync();

            var invalidProductId = Guid.NewGuid();

            var mockRule = new Mock<ICreateOrderRule<CreateOrderRequest, Guid>>();
            var handler = new CreateOrderHandler(mockRule.Object, context);

            var request = new CreateOrderRequest(
                CustomerId: customerId,
                Items: new[] { new CreateOrdersOrderItem(ItemId: invalidProductId, Quantity: 1) }.ToList()
            );

            await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(request, CancellationToken.None));
            mockRule.Verify(r => r.Apply(It.IsAny<CreateOrderRequest>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
