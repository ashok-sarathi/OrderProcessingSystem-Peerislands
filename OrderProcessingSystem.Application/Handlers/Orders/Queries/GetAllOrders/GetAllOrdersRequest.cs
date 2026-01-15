using MediatR;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Data.Helper.Enums;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders
{
    [ExcludeFromCodeCoverage]
    public record GetAllOrdersRequest(
            OrderStatus? OrderStatus,
            Guid? CustomerId
        ) : IRequest<IList<OrderDetailsDto>>;
}
