using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Tests.Handlers.Customers.Commands.CreateCustomer
{
    public class CreateCustomerHandlerTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Handle_ValidRequest_CreatesCustomerAndReturnsId()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            // create mock context backed by the real in-memory context
            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Customers = realContext.Customers;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var handler = new CreateCustomerHandler(mockContext.Object);

            var request = new CreateCustomerRequest(
                Name: "Jane Doe",
                Email: "jane.doe@example.com",
                Phone: "555-1234",
                PermanentAddress: "100 First St",
                ShippingAddress: "100 First St"
            );

            var createdId = await handler.Handle(request, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, createdId);

            var created = await realContext.Customers.FindAsync(new object[] { createdId }, CancellationToken.None);

            Assert.NotNull(created);
            Assert.Equal(request.Name, created.Name);
            Assert.Equal(request.Email, created.Email);
            Assert.Equal(request.Phone, created.Phone);
            Assert.Equal(request.PermanentAddress, created.PermanentAddress);
            Assert.Equal(request.ShippingAddress, created.ShippingAddress);

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_MultipleCalls_CreateDistinctCustomers()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Customers = realContext.Customers;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var handler = new CreateCustomerHandler(mockContext.Object);

            var req1 = new CreateCustomerRequest("A", "a@example.com", "111", "A addr", "A ship");
            var req2 = new CreateCustomerRequest("B", "b@example.com", "222", "B addr", "B ship");

            var id1 = await handler.Handle(req1, CancellationToken.None);
            var id2 = await handler.Handle(req2, CancellationToken.None);

            Assert.NotEqual(Guid.Empty, id1);
            Assert.NotEqual(Guid.Empty, id2);
            Assert.NotEqual(id1, id2);

            var c1 = await realContext.Customers.FindAsync(new object[] { id1 }, CancellationToken.None);
            var c2 = await realContext.Customers.FindAsync(new object[] { id2 }, CancellationToken.None);

            Assert.NotNull(c1);
            Assert.NotNull(c2);
            Assert.Equal(req1.Name, c1.Name);
            Assert.Equal(req2.Name, c2.Name);

            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeast(2));
        }
    }
}
