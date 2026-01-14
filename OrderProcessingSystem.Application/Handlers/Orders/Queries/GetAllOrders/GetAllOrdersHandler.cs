using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Application.Helper.Extentions;
using OrderProcessingSystem.Data.Contexts;

namespace OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersHandler(OrderProcessingSystemContext context) : IRequestHandler<GetAllOrdersRequest, IAsyncEnumerable<OrderDetailsDto>>
    {
        public async Task<IAsyncEnumerable<OrderDetailsDto>> Handle(GetAllOrdersRequest request, CancellationToken cancellationToken)
        {
            return context.Orders.AsNoTracking()
                .WhereIf(request.OrderStatus is not null, o => o.OrderStatus == request.OrderStatus)
                .WhereIf(request.CustomerId is not null, o => o.CustomerId == request.CustomerId)
                .Select(o => new OrderDetailsDto()
                {
                    OrderId = o.Id,
                    OrderStatus = o.OrderStatus.ToString(),
                    OrderDate = o.OrderDate,
                    LastModifiedDate = o.LastModifiedDate,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer.Name,
                    TotalAmount = o.OrderItems.Sum(oi => oi.Quantity * oi.Price)
                }).AsAsyncEnumerable();
        }
    }
}
