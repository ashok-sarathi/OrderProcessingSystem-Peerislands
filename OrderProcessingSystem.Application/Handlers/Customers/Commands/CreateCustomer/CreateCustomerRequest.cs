using MediatR;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer
{
    [ExcludeFromCodeCoverage]
    public record CreateCustomerRequest(
        string Name,
        string Email,
        string Phone,
        string PermanentAddress,
        string ShippingAddress) : IRequest<Guid>;
}
