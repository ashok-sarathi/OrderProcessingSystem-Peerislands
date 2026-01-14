using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Dtos.Customers
{
    [ExcludeFromCodeCoverage]
    public record CustomerDto(
        Guid Id,
        string Name,
        string Email,
        string Phone,
        string PermanentAddress,
        string ShippingAddress
    );
}
