using OrderProcessingSystem.Data.Helper;
using OrderProcessingSystem.Data.Helper.Enums;

namespace OrderProcessingSystem.Data.Entities
{
    public class Order : CommonEntity
    {
        public OrderStatus OrderStatus { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        public Customer Customer { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
