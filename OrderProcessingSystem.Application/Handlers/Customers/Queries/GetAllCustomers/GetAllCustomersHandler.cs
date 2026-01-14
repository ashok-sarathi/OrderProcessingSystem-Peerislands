using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Dtos.Customers;
using OrderProcessingSystem.Data.Contexts;

namespace OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersHandler(OrderProcessingSystemContext context) : IRequestHandler<GetAllCustomersRequest, IList<CustomerDto>>
    {
        public async Task<IList<CustomerDto>> Handle(GetAllCustomersRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await context.Customers.AsNoTracking().Select(x => new CustomerDto(
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
