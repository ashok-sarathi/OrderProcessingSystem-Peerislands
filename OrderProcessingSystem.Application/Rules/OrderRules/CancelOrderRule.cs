using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Application.Rules.OrderRules.IRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Helper.Enums;
using System.Threading;

namespace OrderProcessingSystem.Application.Rules.OrderRules
{
    public class CancelOrderRule(OrderProcessingSystemContext context) : ICancelOrderRule<UpdateOrderStatusRequest>
    {
        public async Task Apply(UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var order = await context.Orders.FindAsync(new object[] { request.Id }, cancellationToken);

                if (order == null)
                    throw new NotFoundException($"Given order id not found : {request.Id}");

                if (order.OrderStatus == OrderStatus.CANCELED)
                    throw new BadRequestException($"Given Order is alread canceled : {request.Id}");

                if (order.OrderStatus != OrderStatus.PENDING)
                    throw new BadRequestException($"Only orders with status PENDING can be canceled, Given Order  current status is {order.OrderStatus.ToString()} : {request.Id}");

                order.OrderStatus = OrderStatus.CANCELED;
                order.LastModifiedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
