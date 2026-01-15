using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Rules.OrderRules;
using OrderProcessingSystem.Application.Helper.Exceptions;

namespace OrderProcessingSystem.Application.Tests.Rules.OrderRules
{
    public class CancelOrderRuleTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Apply_OrderNotFound_ThrowsNotFoundException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());

            // real context used as backing store
            await using var realContext = new OrderProcessingSystemContext(options);
            // mock context so we can assert or intercept calls if needed
            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            // point Orders to the real backing DbSet
            mockContext.Object.Orders = realContext.Orders;
            // forward SaveChangesAsync to real context
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var rule = new CancelOrderRule(mockContext.Object);
            var request = new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.CANCELED);

            await Assert.ThrowsAsync<NotFoundException>(() => rule.Apply(request, CancellationToken.None));
        }

        [Fact]
        public async Task Apply_OrderAlreadyCanceled_ThrowsBadRequestException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());

            await using var realContext = new OrderProcessingSystemContext(options);
            var id = Guid.NewGuid();
            realContext.Orders.Add(new Order
            {
                Id = id,
                OrderStatus = OrderStatus.CANCELED,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                LastModifiedDate = DateTime.UtcNow.AddDays(-1)
            });
            await realContext.SaveChangesAsync();

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var rule = new CancelOrderRule(mockContext.Object);
            var request = new UpdateOrderStatusRequest(id, OrderStatus.CANCELED);

            await Assert.ThrowsAsync<BadRequestException>(() => rule.Apply(request, CancellationToken.None));
        }

        [Fact]
        public async Task Apply_OrderNotPending_ThrowsBadRequestException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());

            await using var realContext = new OrderProcessingSystemContext(options);
            var id = Guid.NewGuid();
            realContext.Orders.Add(new Order
            {
                Id = id,
                OrderStatus = OrderStatus.PROCESSING,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                LastModifiedDate = DateTime.UtcNow.AddDays(-1)
            });
            await realContext.SaveChangesAsync();

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var rule = new CancelOrderRule(mockContext.Object);
            var request = new UpdateOrderStatusRequest(id, OrderStatus.CANCELED);

            await Assert.ThrowsAsync<BadRequestException>(() => rule.Apply(request, CancellationToken.None));
        }

        [Fact]
        public async Task Apply_OrderPending_SetsStatusToCanceledAndUpdatesLastModifiedDate()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());

            await using var realContext = new OrderProcessingSystemContext(options);
            var id = Guid.NewGuid();

            var originalLastModified = DateTime.UtcNow.AddDays(-2);

            realContext.Orders.Add(new Order
            {
                Id = id,
                OrderStatus = OrderStatus.PENDING,
                OrderDate = DateTime.UtcNow.AddDays(-3),
                LastModifiedDate = originalLastModified
            });
            await realContext.SaveChangesAsync();

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var rule = new CancelOrderRule(mockContext.Object);
            var request = new UpdateOrderStatusRequest(id, OrderStatus.CANCELED);

            await rule.Apply(request, CancellationToken.None);

            var updated = await realContext.Orders.FindAsync(new object[] { id }, CancellationToken.None);

            Assert.NotNull(updated);
            Assert.Equal(OrderStatus.CANCELED, updated.OrderStatus);
            Assert.True(updated.LastModifiedDate > originalLastModified, "LastModifiedDate should be updated to a later UTC time.");

            // optionally verify SaveChangesAsync was invoked on the mocked context
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}
