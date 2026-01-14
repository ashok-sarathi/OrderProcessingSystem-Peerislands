using MediatR;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer
{
    public class CreateCustomerHandler(OrderProcessingSystemContext context) : IRequestHandler<CreateCustomerRequest, Guid>
    {
        public async Task<Guid> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
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
