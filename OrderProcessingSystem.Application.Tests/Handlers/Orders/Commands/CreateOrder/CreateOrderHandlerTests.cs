using Moq;
using Xunit;
using MediatR;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Application.Rules.ProductRules.IProductRules;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;
using OrderProcessingSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Tests.Application.Handlers.Orders
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<ICreateOrderRule<CreateOrderRequest, Guid>> _createOrderRuleMock;
        private readonly Mock<IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>>> _getAllProductsRuleMock;
        private readonly Mock<IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>>> _getAllCustomersRuleMock;

        private readonly CreateOrderHandler _handler;

        public CreateOrderHandlerTests()
        {
            _createOrderRuleMock = new Mock<ICreateOrderRule<CreateOrderRequest, Guid>>();
            _getAllProductsRuleMock = new Mock<IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>>>();
            _getAllCustomersRuleMock = new Mock<IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>>>();

            _handler = new CreateOrderHandler(
                _createOrderRuleMock.Object,
                _getAllProductsRuleMock.Object,
                _getAllCustomersRuleMock.Object
            );
        }

        [Fact]
        public async Task Handle_Should_Return_OrderId_When_Request_Is_Valid()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var productId = Guid.NewGuid();
            var orderId = Guid.NewGuid();

            var request = new CreateOrderRequest(customerId,
                new List<CreateOrdersOrderItem>
                {
                    new(productId, 1)
                });

            var customers = new List<Customer>
            {
                new()
                {
                    Id = customerId,
                    Name = "John",
                    Email = "john@test.com",
                    Phone = "123",
                    PermanentAddress = "Addr",
                    ShippingAddress = "Addr"
                }
            }.AsQueryable();

            var products = new List<Product>
            {
                new()
                {
                    Id = productId,
                    Name = "Product",
                    Description = "Desc",
                    Price = 10
                }
            }.AsQueryable();

            _getAllCustomersRuleMock
                .Setup(x => x.Apply(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            _getAllProductsRuleMock
                .Setup(x => x.Apply(It.IsAny<GetAllProductsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            _createOrderRuleMock
                .Setup(x => x.Apply(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(orderId);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal(orderId, result);
        }

        [Fact]
        public async Task Handle_Should_Throw_BadRequest_When_Customer_Is_Invalid()
        {
            // Arrange
            var request = new CreateOrderRequest
            (
                Guid.NewGuid(),
                new List<CreateOrdersOrderItem>()
            );

            _getAllCustomersRuleMock
                .Setup(x => x.Apply(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Customer>().AsQueryable());

            _getAllProductsRuleMock
                .Setup(x => x.Apply(It.IsAny<GetAllProductsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Product>().AsQueryable());

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_Should_Throw_BadRequest_When_Product_Is_Invalid()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var invalidProductId = Guid.NewGuid();

            var request = new CreateOrderRequest
            (
                customerId,
                new List<CreateOrdersOrderItem>
                {
                    new(invalidProductId, 1)
                }
            );

            var customers = new List<Customer>
            {
                new()
                {
                    Id = customerId,
                    Name = "John",
                    Email = "john@test.com",
                    Phone = "123",
                    PermanentAddress = "Addr",
                    ShippingAddress = "Addr"
                }
            }.AsQueryable();

            _getAllCustomersRuleMock
                .Setup(x => x.Apply(It.IsAny<GetAllCustomersRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(customers);

            _getAllProductsRuleMock
                .Setup(x => x.Apply(It.IsAny<GetAllProductsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<Product>().AsQueryable());

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }
    }
}
