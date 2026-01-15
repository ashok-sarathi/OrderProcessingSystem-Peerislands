using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Rules.OrderRules;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Application.Tests.Rules.OrderRules
{
    public class CreateOrderRuleTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Apply_ValidRequest_CreatesOrderAndReturnsId()
        {
            var dbName = Guid.NewGuid().ToString();
            var options = CreateOptions(dbName);

            await using var realContext = new OrderProcessingSystemContext(options);

            // Seed product that will be referenced by the order item
            var productId = Guid.NewGuid();
            var productPrice = 19.99m;
            realContext.Products.Add(new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "desc",
                Price = productPrice
            });
            await realContext.SaveChangesAsync();

            // create a mock context that forwards reads/writes to the real in-memory context
            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Products = realContext.Products;
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var request = new CreateOrderRequest(
                CustomerId: Guid.NewGuid(),
                Items: new[]
                {
                    new CreateOrdersOrderItem(ItemId: productId, Quantity: 2)
                }.ToList()
            );

            var rule = new CreateOrderRule(mockContext.Object);

            var createdOrderId = await rule.Apply(request, CancellationToken.None);

            var created = await realContext.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == createdOrderId, CancellationToken.None);

            Assert.NotNull(created);
            Assert.Equal(request.CustomerId.Value, created.CustomerId);
            Assert.Equal(OrderStatus.PENDING, created.OrderStatus);
            Assert.Equal(1, created.OrderItems.Count);
            var item = created.OrderItems.First();
            Assert.Equal(productId, item.ItemId);
            Assert.Equal(request.Items.First().Quantity, item.Quantity);
            Assert.Equal(productPrice, item.Price);
            Assert.Equal(created.OrderDate, created.LastModifiedDate);

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Apply_NullCustomerId_ThrowsBadRequestException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Products = realContext.Products;
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var request = new CreateOrderRequest(
                CustomerId: null,
                Items: new[]
                {
                    new CreateOrdersOrderItem(ItemId: Guid.NewGuid(), Quantity: 1)
                }.ToList()
            );

            var rule = new CreateOrderRule(mockContext.Object);

            await Assert.ThrowsAsync<BadRequestException>(() => rule.Apply(request, CancellationToken.None));
        }

        [Fact]
        public async Task Apply_NullItemId_ThrowsBadRequestException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Products = realContext.Products;
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var request = new CreateOrderRequest(
                CustomerId: Guid.NewGuid(),
                Items: new[]
                {
                    new CreateOrdersOrderItem(ItemId: null, Quantity: 1)
                }.ToList()
            );

            var rule = new CreateOrderRule(mockContext.Object);

            await Assert.ThrowsAsync<BadRequestException>(() => rule.Apply(request, CancellationToken.None));
        }

        [Fact]
        public async Task Apply_ItemProductMissing_ThrowsInvalidOperationException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Products = realContext.Products;
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            // Do NOT seed the product to simulate missing product
            var missingProductId = Guid.NewGuid();

            var request = new CreateOrderRequest(
                CustomerId: Guid.NewGuid(),
                Items: new[]
                {
                    new CreateOrdersOrderItem(ItemId: missingProductId, Quantity: 1)
                }.ToList()
            );

            var rule = new CreateOrderRule(mockContext.Object);

            // products.First(...) call will throw InvalidOperationException when product not found
            await Assert.ThrowsAsync<InvalidOperationException>(() => rule.Apply(request, CancellationToken.None));
        }
    }
}
