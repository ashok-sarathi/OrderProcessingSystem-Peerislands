using MediatR;
using OrderProcessingSystem.Data.Helper.Enums;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus
{
    [ExcludeFromCodeCoverage]
    public record UpdateOrderStatusRequest(Guid Id, OrderStatus NewStatus) : IRequest;
}
