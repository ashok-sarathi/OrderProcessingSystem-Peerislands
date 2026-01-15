using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
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
    public class CreateCustomerRule(OrderProcessingSystemContext context) : ICreateCustomerRule<CreateCustomerRequest, Guid>
    {
        public async Task<Guid> Apply(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
			try
			{
                var customer = new Customer()
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Email = request.Email,
                    Phone = request.Phone,
                    PermanentAddress = request.PermanentAddress,
                    ShippingAddress = request.ShippingAddress,
                };

                context.Customers.Add(customer);
                await context.SaveChangesAsync(cancellationToken);

                return customer.Id;
            }
			catch
			{
				throw;
			}
        }
    }
}
