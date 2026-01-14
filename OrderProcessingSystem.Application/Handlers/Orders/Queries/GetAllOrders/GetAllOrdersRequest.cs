using MediatR;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Data.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders
{
    public record GetAllOrdersRequest(
            OrderStatus? OrderStatus,
            Guid? CustomerId
        ) : IRequest<IAsyncEnumerable<OrderDetailsDto>>;
}
