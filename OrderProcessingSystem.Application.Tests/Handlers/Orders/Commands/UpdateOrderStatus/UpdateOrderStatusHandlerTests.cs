using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Application.Tests.Handlers.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusHandlerTests
    {
        [Fact]
        public async Task Handle_WhenStatusIsCanceled_ResolvesCancelRuleAndInvokesApply()
        {
            var request = new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.CANCELED);

            var mockCancelRule = new Mock<ICancelOrderRule<UpdateOrderStatusRequest>>();
            mockCancelRule
                .Setup(r => r.Apply(It.IsAny<UpdateOrderStatusRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mockUpdateRule = new Mock<IUpdateOrderRule<UpdateOrderStatusRequest>>();
            mockUpdateRule
                .Setup(r => r.Apply(It.IsAny<UpdateOrderStatusRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockProvider = new Mock<IServiceProvider>();
            mockProvider
                .Setup(p => p.GetService(typeof(ICancelOrderRule<UpdateOrderStatusRequest>)))
                .Returns(mockCancelRule.Object);
            mockProvider
                .Setup(p => p.GetService(typeof(IUpdateOrderRule<UpdateOrderStatusRequest>)))
                .Returns(mockUpdateRule.Object);

            var handler = new UpdateOrderStatusHandler(mockProvider.Object);

            await handler.Handle(request, CancellationToken.None);

            mockCancelRule.Verify(r => r.Apply(It.Is<UpdateOrderStatusRequest>(req => req == request), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenStatusIsNotCanceled_ResolvesUpdateRuleAndInvokesApply()
        {
            var request = new UpdateOrderStatusRequest(Guid.NewGuid(), OrderStatus.SHIPPED);

            var mockCancelRule = new Mock<ICancelOrderRule<UpdateOrderStatusRequest>>();
            mockCancelRule
                .Setup(r => r.Apply(It.IsAny<UpdateOrderStatusRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var mockUpdateRule = new Mock<IUpdateOrderRule<UpdateOrderStatusRequest>>();
            mockUpdateRule
                .Setup(r => r.Apply(It.IsAny<UpdateOrderStatusRequest>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var mockProvider = new Mock<IServiceProvider>();
            mockProvider
                .Setup(p => p.GetService(typeof(ICancelOrderRule<UpdateOrderStatusRequest>)))
                .Returns(mockCancelRule.Object);
            mockProvider
                .Setup(p => p.GetService(typeof(IUpdateOrderRule<UpdateOrderStatusRequest>)))
                .Returns(mockUpdateRule.Object);

            var handler = new UpdateOrderStatusHandler(mockProvider.Object);

            await handler.Handle(request, CancellationToken.None);

            mockUpdateRule.Verify(r => r.Apply(It.Is<UpdateOrderStatusRequest>(req => req == request), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
