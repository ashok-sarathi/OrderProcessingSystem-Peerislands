using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Contexts;

namespace OrderProcessingSystem.Application.Rules.OrderRules
{
    public class UpdateStatusRule(OrderProcessingSystemContext context) : IUpdateOrderRule<UpdateOrderStatusRequest>
    {
        public async Task Apply(UpdateOrderStatusRequest request, CancellationToken cancellationToken)
        {
            var order = await context.Orders.FindAsync(new object[] { request.Id }, cancellationToken);

            if (order == null)
                throw new NotFoundException($"Given order id not found : {request.Id}");

            order.OrderStatus = request.NewStatus;
            order.LastModifiedDate = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }
    }
}
