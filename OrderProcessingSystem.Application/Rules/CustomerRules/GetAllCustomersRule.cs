using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.CustomerRules
{
    public class GetAllCustomersRule(OrderProcessingSystemContext context) : IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>>
    {
        public async Task<IQueryable<Customer>> Apply(GetAllCustomersRequest request, CancellationToken cancellationToken)
        {
            return context.Customers.AsNoTracking();
        }
    }
}
