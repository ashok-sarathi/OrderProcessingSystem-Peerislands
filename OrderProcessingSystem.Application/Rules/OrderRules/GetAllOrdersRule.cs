using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Helper.Extentions;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Rules.OrderRules
{
    public class GetAllOrdersRule(OrderProcessingSystemContext context) : IGetAllOrdersRule<GetAllOrdersRequest, IQueryable<Order>>
    {
        public async Task<IQueryable<Order>> Apply(GetAllOrdersRequest request, CancellationToken cancellationToken)
        {
            return context.Orders.AsNoTracking()
                .WhereIf(request.OrderStatus is not null, o => o.OrderStatus == request.OrderStatus)
                .WhereIf(request.CustomerId is not null, o => o.CustomerId == request.CustomerId);
        }
    }
}
