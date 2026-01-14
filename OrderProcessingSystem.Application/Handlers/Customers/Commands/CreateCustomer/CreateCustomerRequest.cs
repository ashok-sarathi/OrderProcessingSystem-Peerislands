using MediatR;

namespace OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer
{
    public record CreateCustomerRequest(
        string Name,
        string Email,
        string Phone,
        string PermanentAddress,
        string ShippingAddress) : IRequest<Guid>;
}
