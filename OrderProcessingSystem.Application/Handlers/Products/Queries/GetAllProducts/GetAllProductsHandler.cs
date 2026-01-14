using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Dtos.Products;
using OrderProcessingSystem.Data.Contexts;

namespace OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts
{
    public class GetAllProductsHandler(OrderProcessingSystemContext context) : IRequestHandler<GetAllProductsRequest, IList<ProductDto>>
    {
        public async Task<IList<ProductDto>> Handle(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await context.Products.AsNoTracking().Select(x => new ProductDto(
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
