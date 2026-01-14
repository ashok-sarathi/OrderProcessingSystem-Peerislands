using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Dtos.Orders
{
    [ExcludeFromCodeCoverage]
    public record OrderDetailsItemDto
    {
        public Guid ItemId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
