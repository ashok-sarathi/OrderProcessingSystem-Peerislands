using MediatR;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;
using Xunit;
using System.Collections;
using System.Linq.Expressions;

namespace OrderProcessingSystem.Tests.Application.Handlers.Orders.Queries
{
    public class GetAllOrdersHandlerTests
    {
        private readonly Mock<IGetAllOrdersRule<GetAllOrdersRequest, IQueryable<Order>>> _ruleMock;
        private readonly GetAllOrdersHandler _handler;

        public GetAllOrdersHandlerTests()
        {
            _ruleMock = new Mock<IGetAllOrdersRule<GetAllOrdersRequest, IQueryable<Order>>>(MockBehavior.Strict);
            _handler = new GetAllOrdersHandler(_ruleMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnListOfOrderDetailsDto()
        {
            // Arrange
            var request = new GetAllOrdersRequest(null, null);

            var orders = new List<Order>
            {
                new Order
                {
                    Id = Guid.NewGuid(),
                    OrderStatus = OrderStatus.PROCESSING,
                    OrderDate = DateTime.UtcNow.AddDays(-1),
                    LastModifiedDate = DateTime.UtcNow,
                    CustomerId = Guid.NewGuid(),
                    Customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        Name = "John Doe",
                        Email = "john@test.com",
                        Phone = "9999999999",
                        PermanentAddress = "Chennai",
                        ShippingAddress = "Chennai"
                    },
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Quantity = 2, Price = 100 },
                        new OrderItem { Quantity = 1, Price = 50 }
                    }
                },
                new Order
                {
                    Id = Guid.NewGuid(),
                    OrderStatus = OrderStatus.SHIPPED,
                    OrderDate = DateTime.UtcNow.AddDays(-2),
                    LastModifiedDate = DateTime.UtcNow,
                    CustomerId = Guid.NewGuid(),
                    Customer = new Customer
                    {
                        Id = Guid.NewGuid(),
                        Name = "Jane Smith",
                        Email = "jane@test.com",
                        Phone = "8888888888",
                        PermanentAddress = "Bangalore",
                        ShippingAddress = "Bangalore"
                    },
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { Quantity = 3, Price = 200 }
                    }
                }
            };

            var asyncQueryable = new TestAsyncEnumerable<Order>(orders);

            _ruleMock
                .Setup(x => x.Apply(
                    request,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(asyncQueryable);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Collection(result,
                first =>
                {
                    Assert.Equal(orders[0].Id, first.OrderId);
                    Assert.Equal("PROCESSING", first.OrderStatus);
                    Assert.Equal("John Doe", first.CustomerName);
                    Assert.Equal(250, first.TotalAmount);
                },
                second =>
                {
                    Assert.Equal(orders[1].Id, second.OrderId);
                    Assert.Equal("SHIPPED", second.OrderStatus);
                    Assert.Equal("Jane Smith", second.CustomerName);
                    Assert.Equal(600, second.TotalAmount);
                });

            _ruleMock.VerifyAll();
        }
    }

    #region EF Core Async Helpers

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression) { }

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
