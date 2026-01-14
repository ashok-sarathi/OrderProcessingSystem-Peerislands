using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Dtos.Products
{
    [ExcludeFromCodeCoverage]
    public record ProductDto(
        Guid Id,
        string Name,
        string Description,
        decimal Price
    );
}
