using MediatR;
using Moq;
using OrderProcessingSystem.Application.Dtos.Products;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Rules.ProductRules.IProductRules;
using OrderProcessingSystem.Data.Entities;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Tests.Application.Handlers.Products.Queries
{
    public class GetAllProductsHandlerTests
    {
        private readonly Mock<IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>>> _ruleMock;
        private readonly GetAllProductsHandler _handler;

        public GetAllProductsHandlerTests()
        {
            _ruleMock = new Mock<IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>>>(MockBehavior.Strict);
            _handler = new GetAllProductsHandler(_ruleMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfProductDto()
        {
            // Arrange
            var request = new GetAllProductsRequest();

            var products = new List<Product>
            {
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 1",
                    Description = "Desc 1",
                    Price = 100
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Product 2",
                    Description = "Desc 2",
                    Price = 200
                }
            };

            var asyncQueryable = new TestAsyncEnumerable<Product>(products);

            _ruleMock
                .Setup(x => x.Apply(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncQueryable);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Collection(result,
                item =>
                {
                    Assert.Equal(products[0].Id, item.Id);
                    Assert.Equal(products[0].Name, item.Name);
                    Assert.Equal(products[0].Description, item.Description);
                    Assert.Equal(products[0].Price, item.Price);
                },
                item =>
                {
                    Assert.Equal(products[1].Id, item.Id);
                    Assert.Equal(products[1].Name, item.Name);
                    Assert.Equal(products[1].Description, item.Description);
                    Assert.Equal(products[1].Price, item.Price);
                });

            _ruleMock.VerifyAll();
        }
    }

    #region EF Core Async Test Helpers

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());

        IQueryProvider IQueryable.Provider
            => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
            => new ValueTask<bool>(_inner.MoveNext());
    }

    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
            => new TestAsyncEnumerable<TEntity>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new TestAsyncEnumerable<TElement>(expression);

        public object Execute(Expression expression)
            => _inner.Execute(expression)!;

        public TResult Execute<TResult>(Expression expression)
            => _inner.Execute<TResult>(expression)!;

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            => Execute<TResult>(expression);
    }

    #endregion
}
