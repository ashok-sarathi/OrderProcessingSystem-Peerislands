using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.OrderRules
{
    public class GetOrderByIdRule(OrderProcessingSystemContext context) : IGetOrderByIdRule<GetOrderByIdRequest, Order?>
    {
        public async Task<Order?> Apply(GetOrderByIdRequest request, CancellationToken cancellationToken)
        {
            return context.Orders.AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
                .FirstOrDefault(o => o.Id == request.OrderId);
        }
    }
}
