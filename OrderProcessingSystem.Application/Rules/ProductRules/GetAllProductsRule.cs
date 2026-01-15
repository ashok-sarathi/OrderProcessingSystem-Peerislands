using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Rules.ProductRules.IProductRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.ProductRules
{
    public class GetAllProductsRule(OrderProcessingSystemContext context) : IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>>
    {
        public async Task<IQueryable<Product>> Apply(GetAllProductsRequest request, CancellationToken cancellationToken)
        {
            return context.Products.AsNoTracking();
        }
    }
}
