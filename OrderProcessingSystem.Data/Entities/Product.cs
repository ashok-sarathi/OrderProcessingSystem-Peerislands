using OrderProcessingSystem.Data.Helper;

namespace OrderProcessingSystem.Data.Entities
{
    public class Product : CommonEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required decimal Price { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
