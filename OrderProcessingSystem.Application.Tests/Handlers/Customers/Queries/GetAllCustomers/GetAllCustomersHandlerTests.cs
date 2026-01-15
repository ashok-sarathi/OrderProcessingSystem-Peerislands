using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using OrderProcessingSystem.Application.Dtos.Customers;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Tests.Handlers.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersHandlerTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Handle_WhenCustomersExist_ReturnsAllCustomersAsDtos()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var c1 = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Alice",
                Email = "alice@example.com",
                Phone = "111-222-3333",
                PermanentAddress = "123 Main St",
                ShippingAddress = "123 Main St"
            };
            var c2 = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Bob",
                Email = "bob@example.com",
                Phone = "444-555-6666",
                PermanentAddress = "456 Oak Ave",
                ShippingAddress = "456 Oak Ave"
            };

            realContext.Customers.AddRange(c1, c2);
            await realContext.SaveChangesAsync();

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Customers = realContext.Customers;

            var handler = new GetAllCustomersHandler(mockContext.Object);
            var result = await handler.Handle(new GetAllCustomersRequest(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var dto1 = result.Single(r => r.Id == c1.Id);
            Assert.Equal(c1.Name, dto1.Name);
            Assert.Equal(c1.Email, dto1.Email);
            Assert.Equal(c1.Phone, dto1.Phone);
            Assert.Equal(c1.PermanentAddress, dto1.PermanentAddress);
            Assert.Equal(c1.ShippingAddress, dto1.ShippingAddress);

            var dto2 = result.Single(r => r.Id == c2.Id);
            Assert.Equal(c2.Name, dto2.Name);
            Assert.Equal(c2.Email, dto2.Email);
            Assert.Equal(c2.Phone, dto2.Phone);
            Assert.Equal(c2.PermanentAddress, dto2.PermanentAddress);
            Assert.Equal(c2.ShippingAddress, dto2.ShippingAddress);
        }

        [Fact]
        public async Task Handle_WhenNoCustomers_ReturnsEmptyList()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Customers = realContext.Customers;

            var handler = new GetAllCustomersHandler(mockContext.Object);
            var result = await handler.Handle(new GetAllCustomersRequest(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
