using OrderProcessingSystem.Application.Rules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.ProductRules.IProductRules
{
    public interface IGetAllProductsRule<TRequest, TResponse> : ICommonRule<TRequest, TResponse>
    {
    }
}
