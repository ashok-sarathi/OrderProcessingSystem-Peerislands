using OrderProcessingSystem.Data.Helper;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Data.Entities
{
    [ExcludeFromCodeCoverage]
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
