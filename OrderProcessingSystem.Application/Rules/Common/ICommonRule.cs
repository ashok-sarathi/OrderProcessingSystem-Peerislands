using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.Common
{
    public interface ICommonRule<TRequest, TResponse>
    {
        Task<TResponse> Apply(TRequest request, CancellationToken cancellationToken);
    }
}
