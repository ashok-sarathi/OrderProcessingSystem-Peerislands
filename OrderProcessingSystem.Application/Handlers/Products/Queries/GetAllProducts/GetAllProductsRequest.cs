using MediatR;
using OrderProcessingSystem.Application.Dtos.Products;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts
{
    [ExcludeFromCodeCoverage]
    public record GetAllProductsRequest() : IRequest<IList<ProductDto>>;
}
