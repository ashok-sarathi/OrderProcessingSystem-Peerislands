using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Rules.ProductRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using Xunit;

namespace OrderProcessingSystem.Tests.Application.Rules.ProductRules
{
    public class GetAllProductsRuleTests
    {
        private OrderProcessingSystemContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<OrderProcessingSystemContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new OrderProcessingSystemContext(options);
        }

        private static Product CreateProduct(string name, decimal price)
        {
            return new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = "Test product",
                Price = price
            };
        }

        [Fact]
        public async Task Apply_ShouldReturnAllProducts_WhenProductsExist()
        {
            // Arrange
            var context = CreateDbContext();

            var product1 = CreateProduct("Laptop", 50000);
            var product2 = CreateProduct("Mouse", 500);

            context.Products.AddRange(product1, product2);
            await context.SaveChangesAsync();

            var rule = new GetAllProductsRule(context);
            var request = new GetAllProductsRequest();

            // Act
            var result = await rule.Apply(request, CancellationToken.None);
            var products = result.ToList();

            // Assert
            Assert.Equal(2, products.Count);
            Assert.Contains(products, p => p.Name == "Laptop");
            Assert.Contains(products, p => p.Name == "Mouse");
        }

        [Fact]
        public async Task Apply_ShouldReturnEmptyQueryable_WhenNoProductsExist()
        {
            // Arrange
            var context = CreateDbContext();
            var rule = new GetAllProductsRule(context);

            var request = new GetAllProductsRequest();

            // Act
            var result = await rule.Apply(request, CancellationToken.None);
            var products = result.ToList();

            // Assert
            Assert.NotNull(products);
            Assert.Empty(products);
        }
    }
}
