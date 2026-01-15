using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;

namespace OrderProcessingSystem.Application.Tests.Handlers.Customers.Commands.CreateCustomer
{
    public class CreateCustomerHandlerTests
    {
        [Fact]
        public async Task Handle_ValidRequest_ReturnsCreatedId()
        {
            // arrange
            var expectedId = Guid.NewGuid();
            var mockRule = new Mock<ICreateCustomerRule<CreateCustomerRequest, Guid>>();
            mockRule
                .Setup(r => r.Apply(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedId);

            var handler = new CreateCustomerHandler(mockRule.Object);

            var request = new CreateCustomerRequest(
                Name: "Jane Doe",
                Email: "jane.doe@example.com",
                Phone: "555-1234",
                PermanentAddress: "100 First St",
                ShippingAddress: "100 First St"
            );

            // act
            var result = await handler.Handle(request, CancellationToken.None);

            // assert
            Assert.Equal(expectedId, result);
            mockRule.Verify(r => r.Apply(It.Is<CreateCustomerRequest>(rq => rq == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RuleThrows_PropagatesException()
        {
            // arrange
            var mockRule = new Mock<ICreateCustomerRule<CreateCustomerRequest, Guid>>();
            mockRule
                .Setup(r => r.Apply(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("creation failed"));

            var handler = new CreateCustomerHandler(mockRule.Object);

            var request = new CreateCustomerRequest("X", "x@x.com", "111", "perm", "ship");

            // act / assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(request, CancellationToken.None));
            mockRule.Verify(r => r.Apply(It.IsAny<CreateCustomerRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
