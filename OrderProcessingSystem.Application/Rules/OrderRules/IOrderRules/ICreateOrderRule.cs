using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
using OrderProcessingSystem.Application.Rules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules
{
    public interface ICreateOrderRule<TRequest, TResponse> : ICommonRule<TRequest, TResponse>
    {
    }
}
