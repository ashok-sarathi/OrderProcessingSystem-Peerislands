using MediatR;
using OrderProcessingSystem.Application.Dtos.Orders;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById
{
    [ExcludeFromCodeCoverage]
    public record GetOrderByIdRequest(
            Guid OrderId
        ) : IRequest<OrderDetailsDto>;
}
