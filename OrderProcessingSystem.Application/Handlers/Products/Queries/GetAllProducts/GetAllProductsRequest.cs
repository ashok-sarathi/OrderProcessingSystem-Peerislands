using MediatR;
using OrderProcessingSystem.Application.Dtos.Products;

namespace OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts
{
    public record GetAllProductsRequest() : IRequest<IList<ProductDto>>;
}
