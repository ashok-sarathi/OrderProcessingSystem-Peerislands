namespace OrderProcessingSystem.Application.Dtos.Orders
{
    public record OrderDetailsDto
    {
        public Guid OrderId { get; set; }
        public string? OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public IList<OrderDetailsItemDto>? Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }
        public string? ShippingAddress { get; set; }
    }
}
