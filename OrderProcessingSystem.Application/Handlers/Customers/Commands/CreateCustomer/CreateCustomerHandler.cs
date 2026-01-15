using MediatR;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;

namespace OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer
{
    public class CreateCustomerHandler(ICreateCustomerRule<CreateCustomerRequest, Guid> createCustomerRule) : IRequestHandler<CreateCustomerRequest, Guid>
    {
        public async Task<Guid> Handle(CreateCustomerRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await createCustomerRule.Apply(request, cancellationToken);
            }
            catch
            {
                throw;
            }
        }
    }
}
