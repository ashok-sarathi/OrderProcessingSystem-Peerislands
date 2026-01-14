using OrderProcessingSystem.Application.Rules.OrderRules.IRules.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.OrderRules.IRules
{
    public interface ICancelOrderRule<T> : ICommonVoidRule<T>
    {
    }
}
