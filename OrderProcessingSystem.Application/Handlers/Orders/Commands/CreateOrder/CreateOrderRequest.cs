using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder
{
    [ExcludeFromCodeCoverage]
    public record CreateOrderRequest(
            Guid? CustomerId,
            IList<CreateOrdersOrderItem> Items
        ) : IRequest<Guid>;

    [ExcludeFromCodeCoverage]
    public record CreateOrdersOrderItem(
            Guid? ItemId,
            int Quantity
        );
}
