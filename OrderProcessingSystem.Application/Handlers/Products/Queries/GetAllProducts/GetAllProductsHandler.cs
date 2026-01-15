using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Dtos.Products;
using OrderProcessingSystem.Application.Rules.ProductRules.IProductRules;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts
{
    public class GetAllProductsHandler(IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>> getAllProductsRule) : IRequestHandler<GetAllProductsRequest, IList<ProductDto>>
    {
        public async Task<IList<ProductDto>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await getAllProductsRule.Apply(request, cancellationToken);
                return await result.Select(x => new ProductDto(
                        x.Id,
                        x.Name,
                        x.Description,
                        x.Price
                    )).ToListAsync(cancellationToken: cancellationToken);
            }
            catch
            {
                throw;
            }
        }
    }
}
