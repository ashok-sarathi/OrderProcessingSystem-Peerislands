using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using OrderProcessingSystem.Api.Controllers;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Dtos.Products;

namespace OrderProcessingSystem.Api.Tests.Controllers
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task Get_ReturnsOk_WithProductList()
        {
            // arrange
            var expected = new List<ProductDto>
            {
                new ProductDto(Guid.NewGuid(), "P1", "D1", 1.1m),
                new ProductDto(Guid.NewGuid(), "P2", "D2", 2.2m)
            };

            var mockSender = new Mock<ISender>();
            mockSender
                .Setup(s => s.Send(It.IsAny<GetAllProductsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expected);

            var controller = new ProductsController(mockSender.Object);

            // act
            var actionResult = await controller.Get();

            // assert
            var ok = Assert.IsType<OkObjectResult>(actionResult);
            Assert.Same(expected, Assert.IsType<List<ProductDto>>(ok.Value));
        }

        [Fact]
        public async Task Get_WhenSenderThrows_PropagatesException()
        {
            var mockSender = new Mock<ISender>();
            mockSender
                .Setup(s => s.Send(It.IsAny<GetAllProductsRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("boom"));

            var controller = new ProductsController(mockSender.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => controller.Get());
        }
    }
}
