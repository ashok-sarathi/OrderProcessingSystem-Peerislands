using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Application.Rules.Common;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusHandler(IServiceProvider serviceProvider) : IRequestHandler<UpdateOrderStatusRequest>
    {
        public async Task Handle(UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ICommonVoidRule<UpdateOrderStatusRequest> rule = request.NewStatus switch
                {
                    OrderStatus.CANCELED => serviceProvider.GetRequiredService<ICancelOrderRule<UpdateOrderStatusRequest>>(),
                    _ => serviceProvider.GetRequiredService<IUpdateOrderRule<UpdateOrderStatusRequest>>(),
                };

                await rule.Apply(request, cancellationToken);
            }
            catch
            {
                throw;
            }
        }
    }
}
