using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using OrderProcessingSystem.Application.Dtos.Customers;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace OrderProcessingSystem.Application.Tests.Handlers.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersHandlerTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Handle_WhenNoCustomers_ReturnsEmptyList()
        {
            // arrange
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockRule = new Mock<IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>>>();
            mockRule
                .Setup(r => r.Apply(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IQueryable<Customer>)realContext.Customers);

            var handler = new GetAllCustomersHandler(mockRule.Object);

            // act
            var result = await handler.Handle(new GetAllCustomersRequest(), CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockRule.Verify(r => r.Apply(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithCustomers_ReturnsMappedCustomerDtos()
        {
            // arrange
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var c1 = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Alice",
                Email = "alice@example.com",
                Phone = "111-222",
                PermanentAddress = "Perm A",
                ShippingAddress = "Ship A"
            };
            var c2 = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Bob",
                Email = "bob@example.com",
                Phone = "333-444",
                PermanentAddress = "Perm B",
                ShippingAddress = "Ship B"
            };

            realContext.Customers.AddRange(c1, c2);
            await realContext.SaveChangesAsync();

            var mockRule = new Mock<IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>>>();
            mockRule
                .Setup(r => r.Apply(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((IQueryable<Customer>)realContext.Customers);

            var handler = new GetAllCustomersHandler(mockRule.Object);

            // act
            var result = await handler.Handle(new GetAllCustomersRequest(), CancellationToken.None);

            // assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var dto1 = result.Single(d => d.Id == c1.Id);
            Assert.Equal(c1.Name, dto1.Name);
            Assert.Equal(c1.Email, dto1.Email);
            Assert.Equal(c1.Phone, dto1.Phone);
            Assert.Equal(c1.PermanentAddress, dto1.PermanentAddress);
            Assert.Equal(c1.ShippingAddress, dto1.ShippingAddress);

            var dto2 = result.Single(d => d.Id == c2.Id);
            Assert.Equal(c2.Name, dto2.Name);
            Assert.Equal(c2.Email, dto2.Email);
            Assert.Equal(c2.Phone, dto2.Phone);

            mockRule.Verify(r => r.Apply(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
