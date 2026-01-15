using MediatR;
using OrderProcessingSystem.Application.Dtos.Customers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers
{
    [ExcludeFromCodeCoverage]
    public record GetAllCustomersRequest : IRequest<IList<CustomerDto>>;
}
