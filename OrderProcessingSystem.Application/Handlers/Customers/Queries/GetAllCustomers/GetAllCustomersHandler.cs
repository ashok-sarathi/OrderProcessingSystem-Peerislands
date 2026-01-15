using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Dtos.Customers;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersHandler(IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>> getAllCustomersRule) : IRequestHandler<GetAllCustomersRequest, IList<CustomerDto>>
    {
        public async Task<IList<CustomerDto>> Handle(GetAllCustomersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await getAllCustomersRule.Apply(request, cancellationToken);
                return await result.Select(x => new CustomerDto(
                        x.Id,
                        x.Name,
                        x.Email,
                        x.Phone,
                        x.PermanentAddress,
                        x.ShippingAddress
                    )).ToListAsync(cancellationToken: cancellationToken);
            }
            catch
            {
                throw;
            }
        }
    }
}
