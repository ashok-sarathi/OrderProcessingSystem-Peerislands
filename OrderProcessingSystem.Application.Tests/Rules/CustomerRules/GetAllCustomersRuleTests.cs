using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using OrderProcessingSystem.Application.Rules.CustomerRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using Xunit;

namespace OrderProcessingSystem.Tests.Application.Rules.CustomerRules
{
    public class GetAllCustomersRuleTests
    {
        private OrderProcessingSystemContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new OrderProcessingSystemContext(options);
        }

        [Fact]
        public async Task Apply_ShouldReturnAllCustomers_AsQueryable()
        {
            // Arrange
            var context = CreateDbContext();

            var customers = new List<Customer>
            {
                new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = "John Doe",
                    Email = "john@test.com",
                    Phone = "9999999999",
                    PermanentAddress = "Chennai",
                    ShippingAddress = "Chennai"
                },
                new Customer
                {
                    Id = Guid.NewGuid(),
                    Name = "Jane Smith",
                    Email = "jane@test.com",
                    Phone = "8888888888",
                    PermanentAddress = "Bangalore",
                    ShippingAddress = "Bangalore"
                }
            };

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();

            var rule = new GetAllCustomersRule(context);
            var request = new GetAllCustomersRequest();

            // Act
            var result = await rule.Apply(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IQueryable<Customer>>(result);

            var list = result.ToList();
            Assert.Equal(2, list.Count);

            Assert.Contains(list, c => c.Name == "John Doe");
            Assert.Contains(list, c => c.Name == "Jane Smith");
        }
    }
}
