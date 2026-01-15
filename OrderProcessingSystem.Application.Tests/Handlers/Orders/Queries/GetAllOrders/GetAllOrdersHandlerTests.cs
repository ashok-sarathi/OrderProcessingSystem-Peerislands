using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Application.Tests.Handlers.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandlerTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        private static async Task<List<OrderDetailsDto>> CollectAsync(IAsyncEnumerable<OrderDetailsDto> source, CancellationToken ct = default)
        {
            var list = new List<OrderDetailsDto>();
            await foreach (var item in source.WithCancellation(ct))
                list.Add(item);
            return list;
        }

        [Fact]
        public async Task Handle_WhenNoOrders_ReturnsEmpty()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Object.Customers = realContext.Customers;
            mockContext.Object.OrderItems = realContext.OrderItems;
            mockContext.Object.Products = realContext.Products;

            var handler = new GetAllOrdersHandler(mockContext.Object);

            var result = await handler.Handle(new GetAllOrdersRequest(null, null), CancellationToken.None);
            var list = await CollectAsync(result);

            Assert.NotNull(list);
            Assert.Empty(list);
        }

        [Fact]
        public async Task Handle_WithOrders_ReturnsMappedOrderDetails()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            // seed customer
            var customerId = Guid.NewGuid();
            var customer = new Customer
            {
                Id = customerId,
                Name = "Customer A",
                Email = "a@example.com",
                Phone = "000-111-222",
                PermanentAddress = "Addr1",
                ShippingAddress = "Ship1"
            };
            realContext.Customers.Add(customer);

            // seed product
            var productId = Guid.NewGuid();
            var product = new Product
            {
                Id = productId,
                Name = "Prod A",
                Description = "Desc",
                Price = 10m
            };
            realContext.Products.Add(product);

            // seed order with two items
            var orderId = Guid.NewGuid();
            var order = new Order
            {
                Id = orderId,
                CustomerId = customerId,
                Customer = customer,
                OrderDate = DateTime.UtcNow.AddDays(-1),
                LastModifiedDate = DateTime.UtcNow.AddDays(-1),
                OrderStatus = OrderStatus.PROCESSING,
                OrderItems = new List<OrderItem>()
            };
            var item1 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Order = order,
                ItemId = productId,
                Item = product,
                Quantity = 2,
                Price = product.Price
            };
            var item2 = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                Order = order,
                ItemId = productId,
                Item = product,
                Quantity = 1,
                Price = product.Price
            };
            order.OrderItems.Add(item1);
            order.OrderItems.Add(item2);

            realContext.Orders.Add(order);
            realContext.OrderItems.AddRange(item1, item2);

            await realContext.SaveChangesAsync();

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Customers = realContext.Customers;
            mockContext.Object.Products = realContext.Products;
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Object.OrderItems = realContext.OrderItems;
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns((CancellationToken ct) => realContext.SaveChangesAsync(ct));

            var handler = new GetAllOrdersHandler(mockContext.Object);
            var result = await handler.Handle(new GetAllOrdersRequest(null, null), CancellationToken.None);
            var list = await CollectAsync(result);

            Assert.Single(list);
            var dto = list.Single();
            Assert.Equal(orderId, dto.OrderId);
            Assert.Equal(order.OrderStatus.ToString(), dto.OrderStatus);
            Assert.Equal(customerId, dto.CustomerId);
            Assert.Equal(customer.Name, dto.CustomerName);
            Assert.Equal((item1.Quantity * item1.Price) + (item2.Quantity * item2.Price), dto.TotalAmount);
        }

        [Fact]
        public async Task Handle_WithFilters_FiltersByStatusAndCustomer()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            // customers
            var c1 = new Customer { Id = Guid.NewGuid(), Name = "C1", Email = "c1@x", Phone = "1", PermanentAddress = "a", ShippingAddress = "a" };
            var c2 = new Customer { Id = Guid.NewGuid(), Name = "C2", Email = "c2@x", Phone = "2", PermanentAddress = "b", ShippingAddress = "b" };
            realContext.Customers.AddRange(c1, c2);

            // product
            var product = new Product { Id = Guid.NewGuid(), Name = "P", Description = "d", Price = 5m };
            realContext.Products.Add(product);

            // order for c1 with PENDING
            var o1 = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = c1.Id,
                Customer = c1,
                OrderStatus = OrderStatus.PENDING,
                OrderDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem> {
                    new OrderItem { Id = Guid.NewGuid(), ItemId = product.Id, Item = product, Quantity = 1, Price = product.Price }
                }
            };

            // order for c2 with SHIPPED
            var o2 = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = c2.Id,
                Customer = c2,
                OrderStatus = OrderStatus.SHIPPED,
                OrderDate = DateTime.UtcNow,
                LastModifiedDate = DateTime.UtcNow,
                OrderItems = new List<OrderItem> {
                    new OrderItem { Id = Guid.NewGuid(), ItemId = product.Id, Item = product, Quantity = 2, Price = product.Price }
                }
            };

            realContext.Orders.AddRange(o1, o2);
            await realContext.SaveChangesAsync();

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Customers = realContext.Customers;
            mockContext.Object.Products = realContext.Products;
            mockContext.Object.Orders = realContext.Orders;
            mockContext.Object.OrderItems = realContext.OrderItems;

            var handler = new GetAllOrdersHandler(mockContext.Object);

            // filter by status PENDING => should return only o1
            var resStatus = await handler.Handle(new GetAllOrdersRequest(OrderStatus.PENDING, null), CancellationToken.None);
            var listStatus = await CollectAsync(resStatus);
            Assert.Single(listStatus);
            Assert.Equal(o1.Id, listStatus.Single().OrderId);

            // filter by customer c2 => should return only o2
            var resCustomer = await handler.Handle(new GetAllOrdersRequest(null, c2.Id), CancellationToken.None);
            var listCustomer = await CollectAsync(resCustomer);
            Assert.Single(listCustomer);
            Assert.Equal(o2.Id, listCustomer.Single().OrderId);

            // filter by both status SHIPPED and customer c2 => o2
            var resBoth = await handler.Handle(new GetAllOrdersRequest(OrderStatus.SHIPPED, c2.Id), CancellationToken.None);
            var listBoth = await CollectAsync(resBoth);
            Assert.Single(listBoth);
            Assert.Equal(o2.Id, listBoth.Single().OrderId);
        }
    }
}
