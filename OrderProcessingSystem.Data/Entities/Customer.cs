using OrderProcessingSystem.Data.Helper;

namespace OrderProcessingSystem.Data.Entities
{
    public class Customer : CommonEntity
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public required string PermanentAddress { get; set; }
        public required string ShippingAddress { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
