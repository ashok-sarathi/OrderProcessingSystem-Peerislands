using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder
{
    public record CreateOrderRequest(
            Guid? CustomerId,
            IList<CreateOrdersOrderItem> Items
        ) : IRequest<Guid>;

    public record CreateOrdersOrderItem(
            Guid? ItemId,
            int Quantity
        );
}
