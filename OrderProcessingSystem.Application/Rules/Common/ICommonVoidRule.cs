using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.Common
{
    public interface ICommonVoidRule<T>
    {
        Task Apply(T request, CancellationToken cancellationToken);
    }
}
