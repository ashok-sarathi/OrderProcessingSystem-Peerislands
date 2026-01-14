namespace OrderProcessingSystem.Application.Dtos.Customers
{
    public record CustomerDto(
        Guid Id,
        string Name,
        string Email,
        string Phone,
        string PermanentAddress,
        string ShippingAddress
    );
}
