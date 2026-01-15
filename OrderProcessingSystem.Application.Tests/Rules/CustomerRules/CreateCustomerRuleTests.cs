using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
using OrderProcessingSystem.Application.Rules.CustomerRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using Xunit;

namespace OrderProcessingSystem.Tests.Application.Rules.CustomerRules
{
    public class CreateCustomerRuleTests
    {
        private OrderProcessingSystemContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new OrderProcessingSystemContext(options);
        }

        [Fact]
        public async Task Apply_ShouldCreateCustomer_AndReturnCustomerId()
        {
            // Arrange
            var context = CreateDbContext();
            var rule = new CreateCustomerRule(context);

            var request = new CreateCustomerRequest(
                Name: "John Doe",
                Email: "john@test.com",
                Phone: "9999999999",
                PermanentAddress: "Chennai",
                ShippingAddress: "Chennai"
            );

            // Act
            var customerId = await rule.Apply(request, CancellationToken.None);

            // Assert
            Assert.NotEqual(Guid.Empty, customerId);

            var customer = await context.Customers.FirstOrDefaultAsync();

            Assert.NotNull(customer);
            Assert.Equal(customerId, customer!.Id);
            Assert.Equal("John Doe", customer.Name);
            Assert.Equal("john@test.com", customer.Email);
            Assert.Equal("9999999999", customer.Phone);
            Assert.Equal("Chennai", customer.PermanentAddress);
            Assert.Equal("Chennai", customer.ShippingAddress);
        }
    }
}
