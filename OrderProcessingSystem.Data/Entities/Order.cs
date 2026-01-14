using OrderProcessingSystem.Data.Helper;
using OrderProcessingSystem.Data.Helper.Enums;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Data.Entities
{
    [ExcludeFromCodeCoverage]
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
