using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Application.Tests.Handlers.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandlerTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Handle_OrderNotFound_ThrowsNotFoundException()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());

            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Object.Customers = realContext.Customers;
            mockContext.Object.OrderItems = realContext.OrderItems;
            mockContext.Object.Products = realContext.Products;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var handler = new GetOrderByIdHandler(mockContext.Object);

            var unknownId = Guid.NewGuid();
            var request = new GetOrderByIdRequest(unknownId);

            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_OrderExists_ReturnsMappedOrderDetails()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            // Seed customer
            var customerId = Guid.NewGuid();
            var customer = new Customer
            {
                Id = customerId,
                Name = "Cust Name",
                Email = "cust@example.com",
                Phone = "123-456",
                PermanentAddress = "Perm Addr",
                ShippingAddress = "Ship Addr"
            };
            realContext.Customers.Add(customer);

            // Seed products
            var prod1Id = Guid.NewGuid();
            var prod1 = new Product
            {
                Id = prod1Id,
                Name = "Product 1",
                Description = "Desc 1",
                Price = 12.5m
            };
            var prod2Id = Guid.NewGuid();
            var prod2 = new Product
            {
                Id = prod2Id,
                Name = "Product 2",
                Description = "Desc 2",
                Price = 5m
            };
            realContext.Products.AddRange(prod1, prod2);

            // Create order and items
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                Customer = customer,
                OrderDate = DateTime.UtcNow.AddDays(-2),
                LastModifiedDate = DateTime.UtcNow.AddDays(-1),
                OrderStatus = OrderStatus.SHIPPED,
                OrderItems = new List<OrderItem>()
            };

            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Order = order,
                ItemId = prod1Id,
                Item = prod1,
                Quantity = 2,
                Price = prod1.Price
            };
            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Order = order,
                ItemId = prod2Id,
                Item = prod2,
                Quantity = 3,
                Price = prod2.Price
            };
            order.OrderItems.Add(item1);
            order.OrderItems.Add(item2);

            realContext.Orders.Add(order);
            realContext.OrderItems.AddRange(item1, item2);

            await realContext.SaveChangesAsync();

            // create mock context backed by the real in-memory context
            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Customers = realContext.Customers;
            mockContext.Object.Products = realContext.Products;
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Object.OrderItems = realContext.OrderItems;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                       .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var handler = new GetOrderByIdHandler(mockContext.Object);
            var request = new GetOrderByIdRequest(orderId);

            var dto = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(dto);
            Assert.Equal(orderId, dto.OrderId);
            Assert.Equal(order.OrderStatus.ToString(), dto.OrderStatus);
            Assert.Equal(order.OrderDate, dto.OrderDate);
            Assert.Equal(order.LastModifiedDate, dto.LastModifiedDate);
            Assert.Equal(customerId, dto.CustomerId);
            Assert.Equal(customer.Name, dto.CustomerName);
            Assert.Equal(customer.Email, dto.CustomerEmail);
            Assert.Equal(customer.Phone, dto.CustomerPhone);
            Assert.Equal(customer.ShippingAddress, dto.ShippingAddress);

            Assert.NotNull(dto.Items);
            Assert.Equal(2, dto.Items.Count);

            var dtoItem1 = dto.Items.Single(i => i.ItemId == prod1Id);
            Assert.Equal(prod1.Name, dtoItem1.ProductName);
            Assert.Equal(prod1.Description, dtoItem1.ProductDescription);
            Assert.Equal(item1.Price, dtoItem1.Price);
            Assert.Equal(item1.Quantity, dtoItem1.Quantity);

            var dtoItem2 = dto.Items.Single(i => i.ItemId == prod2Id);
            Assert.Equal(prod2.Name, dtoItem2.ProductName);
            Assert.Equal(prod2.Description, dtoItem2.ProductDescription);
            Assert.Equal(item2.Price, dtoItem2.Price);
            Assert.Equal(item2.Quantity, dtoItem2.Quantity);

            var expectedTotal = (item1.Price * item1.Quantity) + (item2.Price * item2.Quantity);
            Assert.Equal(expectedTotal, dto.TotalAmount);
        }
    }
}
