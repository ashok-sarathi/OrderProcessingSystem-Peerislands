using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandler(IGetAllOrdersRule<GetAllOrdersRequest, IQueryable<Order>> getAllOrdersRule) : IRequestHandler<GetAllOrdersRequest, IList<OrderDetailsDto>>
    {
        public async Task<IList<OrderDetailsDto>> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken)
        {
            var result = await getAllOrdersRule.Apply(request, cancellationToken);
            return await result
                .Select(o => new OrderDetailsDto()
                {
                    OrderId = o.Id,
                    OrderStatus = o.OrderStatus.ToString(),
                    OrderDate = o.OrderDate,
                    LastModifiedDate = o.LastModifiedDate,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.Name,
                    TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.Price)
                }).ToListAsync();
        }
    }
}
