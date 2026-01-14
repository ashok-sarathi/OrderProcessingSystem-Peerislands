using MediatR;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus
{
    public record UpdateOrderStatusRequest(Guid Id, OrderStatus NewStatus) : IRequest;
}
