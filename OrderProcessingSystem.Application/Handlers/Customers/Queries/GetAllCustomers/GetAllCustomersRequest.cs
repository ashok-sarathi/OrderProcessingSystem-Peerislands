using MediatR;
using OrderProcessingSystem.Application.Dtos.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers
{
    public record GetAllCustomersRequest : IRequest<IList<CustomerDto>>;
}
