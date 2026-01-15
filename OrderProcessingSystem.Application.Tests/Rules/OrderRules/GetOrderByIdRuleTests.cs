using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById;
using OrderProcessingSystem.Application.Rules.OrderRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;
using Xunit;

namespace OrderProcessingSystem.Tests.Application.Rules.OrderRules
{
    public class GetOrderByIdRuleTests
    {
        private OrderProcessingSystemContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new OrderProcessingSystemContext(options);
        }

        private static Customer CreateCustomer(string name)
        {
            return new Customer
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = $"{name.ToLower()}@test.com",
                Phone = "9999999999",
                PermanentAddress = "Chennai",
                ShippingAddress = "Chennai"
            };
        }

        private static Order CreateOrder(Guid customerId)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                OrderStatus = OrderStatus.PROCESSING,
                OrderDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };
        }

        [Fact]
        public async Task Apply_ShouldReturnOrder_WithCustomerAndItems_WhenOrderExists()
        {
            // Arrange
            var context = CreateDbContext();

            var customer = CreateCustomer("John Doe");

            var order = CreateOrder(customer.Id);

            order.Customer = customer;
            order.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ItemId = Guid.NewGuid(),
                Quantity = 2,
                Price = 100
            });

            context.Customers.Add(customer);
            context.OrderItems.Add(new OrderItem
            {
                OrderId = order.Id,
                ItemId = Guid.NewGuid(),
                Quantity = 2,
                Price = 100
            });
            context.Orders.Add(order);

            await context.SaveChangesAsync();

            var rule = new GetOrderByIdRule(context);
            var request = new GetOrderByIdRequest(order.Id);

            // Act
            var result = await rule.Apply(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(order.Id, result!.Id);

            // Customer included
            Assert.NotNull(result.Customer);
            Assert.Equal("John Doe", result.Customer.Name);
        }

        [Fact]
        public async Task Apply_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            // Arrange
            var context = CreateDbContext();
            var rule = new GetOrderByIdRule(context);

            var request = new GetOrderByIdRequest(Guid.NewGuid());

            // Act
            var result = await rule.Apply(request, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }
    }
}
