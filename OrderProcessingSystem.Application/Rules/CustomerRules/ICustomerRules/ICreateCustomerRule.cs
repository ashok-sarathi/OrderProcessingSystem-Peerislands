using OrderProcessingSystem.Application.Rules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules
{
    public interface ICreateCustomerRule<TRequest, TResponse> : ICommonRule<TRequest, TResponse>
    {
    }
}
