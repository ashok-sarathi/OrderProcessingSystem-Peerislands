using MediatR;
using OrderProcessingSystem.Application.Dtos.Orders;

namespace OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById
{
    public record GetOrderByIdRequest(
            Guid OrderId
        ) : IRequest<OrderDetailsDto>;
}
