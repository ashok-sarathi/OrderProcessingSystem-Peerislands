using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Rules.OrderRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;
using Xunit;

namespace OrderProcessingSystem.Tests.Application.Rules.OrderRules
{
    public class GetAllOrdersRuleTests
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

        private static Order CreateOrder(Guid customerId, OrderStatus status)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                OrderStatus = status,
                OrderDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { Quantity = 1, Price = 100 }
                }
            };
        }

        [Fact]
        public async Task Apply_ShouldReturnAllOrders_WhenNoFiltersProvided()
        {
            // Arrange
            var context = CreateDbContext();

            var customer1 = CreateCustomer("John");
            var customer2 = CreateCustomer("Jane");

            context.Customers.AddRange(customer1, customer2);

            var orders = new List<Order>
            {
                CreateOrder(customer1.Id, OrderStatus.PENDING),
                CreateOrder(customer2.Id, OrderStatus.SHIPPED)
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            var rule = new GetAllOrdersRule(context);
            var request = new GetAllOrdersRequest(null, null);

            // Act
            var result = await rule.Apply(request, CancellationToken.None);
            var list = result.ToList();

            // Assert
            Assert.Equal(2, list.Count);
        }

        [Fact]
        public async Task Apply_ShouldFilterByOrderStatus()
        {
            // Arrange
            var context = CreateDbContext();

            var customer = CreateCustomer("John");
            context.Customers.Add(customer);

            var orders = new List<Order>
            {
                CreateOrder(customer.Id, OrderStatus.PENDING),
                CreateOrder(customer.Id, OrderStatus.SHIPPED),
                CreateOrder(customer.Id, OrderStatus.PENDING)
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            var rule = new GetAllOrdersRule(context);
            var request = new GetAllOrdersRequest(OrderStatus.PENDING, null);

            // Act
            var result = await rule.Apply(request, CancellationToken.None);
            var list = result.ToList();

            // Assert
            Assert.Equal(2, list.Count);
            Assert.All(list, o => Assert.Equal(OrderStatus.PENDING, o.OrderStatus));
        }

        [Fact]
        public async Task Apply_ShouldFilterByCustomerId()
        {
            // Arrange
            var context = CreateDbContext();

            var customer1 = CreateCustomer("John");
            var customer2 = CreateCustomer("Jane");

            context.Customers.AddRange(customer1, customer2);

            var orders = new List<Order>
            {
                CreateOrder(customer1.Id, OrderStatus.PENDING),
                CreateOrder(customer2.Id, OrderStatus.PENDING),
                CreateOrder(customer1.Id, OrderStatus.SHIPPED)
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            var rule = new GetAllOrdersRule(context);
            var request = new GetAllOrdersRequest(null, customer1.Id);

            // Act
            var result = await rule.Apply(request, CancellationToken.None);
            var list = result.ToList();

            // Assert
            Assert.Equal(2, list.Count);
            Assert.All(list, o => Assert.Equal(customer1.Id, o.CustomerId));
        }

        [Fact]
        public async Task Apply_ShouldApplyBothFilters_WhenProvided()
        {
            // Arrange
            var context = CreateDbContext();

            var customer1 = CreateCustomer("John");
            var customer2 = CreateCustomer("Jane");

            context.Customers.AddRange(customer1, customer2);

            var orders = new List<Order>
            {
                CreateOrder(customer1.Id, OrderStatus.PENDING),
                CreateOrder(customer1.Id, OrderStatus.SHIPPED),
                CreateOrder(customer2.Id, OrderStatus.PENDING)
            };

            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            var rule = new GetAllOrdersRule(context);
            var request = new GetAllOrdersRequest(OrderStatus.PENDING, customer1.Id);

            // Act
            var result = await rule.Apply(request, CancellationToken.None);
            var list = result.ToList();

            // Assert
            Assert.Single(list);
            Assert.Equal(OrderStatus.PENDING, list[0].OrderStatus);
            Assert.Equal(customer1.Id, list[0].CustomerId);
        }
    }
}
