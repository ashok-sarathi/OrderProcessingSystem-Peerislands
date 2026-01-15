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
    public class UpdateStatusRuleTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Apply_OrderNotFound_ThrowsNotFoundException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());

            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var rule = new UpdateStatusRule(mockContext.Object);

            var request = new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.SHIPPED);

            await Assert.ThrowsAsync<NotFoundException>(() => rule.Apply(request, CancellationToken.None));
        }

        [Fact]
        public async Task Apply_OrderExists_UpdatesStatusAndLastModifiedDate()
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

            var rule = new UpdateStatusRule(mockContext.Object);
            var request = new UpdateOrderStatusRequest(id, OrderStatus.SHIPPED);

            await rule.Apply(request, CancellationToken.None);

            var updated = await realContext.Orders.FindAsync(new object[] { id }, CancellationToken.None);

            Assert.NotNull(updated);
            Assert.Equal(OrderStatus.SHIPPED, updated.OrderStatus);
            Assert.True(updated.LastModifiedDate > originalLastModified, "LastModifiedDate should be updated to a later UTC time.");

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}
