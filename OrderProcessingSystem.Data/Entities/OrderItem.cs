using OrderProcessingSystem.Data.Helper;

namespace OrderProcessingSystem.Data.Entities
{
    public class OrderItem : CommonEntity
    {
        public Guid OrderId { get; set; }
        public Guid ItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public Order Order { get; set; }
        public Product Item { get; set; }
    }
}
