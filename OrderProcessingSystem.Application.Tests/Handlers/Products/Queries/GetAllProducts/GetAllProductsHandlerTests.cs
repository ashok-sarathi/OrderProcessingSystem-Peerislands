using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Dtos.Products;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Tests.Products.Queries.GetAllProducts
{
    public class GetAllProductsHandlerTests
    {
        private static DbContextOptions<OrderProcessingSystemContext> CreateOptions(string dbName) =>
            new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

        [Fact]
        public async Task Handle_WhenProductsExist_ReturnsAllProductsAsDtos()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var p1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product A",
                Description = "Desc A",
                Price = 9.99m
            };
            var p2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product B",
                Description = "Desc B",
                Price = 19.50m
            };

            realContext.Products.AddRange(p1, p2);
            await realContext.SaveChangesAsync();

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Products = realContext.Products;

            var handler = new GetAllProductsHandler(mockContext.Object);
            var result = await handler.Handle(new GetAllProductsRequest(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            var dto1 = result.Single(r => r.Id == p1.Id);
            Assert.Equal(p1.Name, dto1.Name);
            Assert.Equal(p1.Description, dto1.Description);
            Assert.Equal(p1.Price, dto1.Price);

            var dto2 = result.Single(r => r.Id == p2.Id);
            Assert.Equal(p2.Name, dto2.Name);
            Assert.Equal(p2.Description, dto2.Description);
            Assert.Equal(p2.Price, dto2.Price);
        }

        [Fact]
        public async Task Handle_WhenNoProducts_ReturnsEmptyList()
        {
            var options = CreateOptions(Guid.NewGuid().ToString());
            await using var realContext = new OrderProcessingSystemContext(options);

            var mockContext = new Mock<OrderProcessingSystemContext>(options) { CallBase = true };
            mockContext.Object.Products = realContext.Products;

            var handler = new GetAllProductsHandler(mockContext.Object);
            var result = await handler.Handle(new GetAllProductsRequest(), CancellationToken.None);

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}