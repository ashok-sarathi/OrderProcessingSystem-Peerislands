using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Contexts;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler(ICreateOrderRule<CreateOrderRequest, Guid> createOrderRule, OrderProcessingSystemContext context) : IRequestHandler<CreateOrderRequest, Guid>
    {
        public async Task<Guid> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ValidateRequest(request);

                return await createOrderRule.Apply(request, cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        private void ValidateRequest(CreateOrderRequest request)
        {
            if (!context.Customers.AsNoTracking().Any(c => c.Id == request.CustomerId))
                throw new BadRequestException($"Invalid customer ID: {request.CustomerId}");

            var products = context.Products.AsNoTracking().Select(p => p.Id);
            var invalidProductIds = request.Items
                .Where(item => item.ItemId.HasValue && !products.Contains(item.ItemId.Value))
                .Select(item => item.ItemId)
                .ToList();
            if (invalidProductIds.Any())
                throw new BadRequestException($"Invalid product IDs: {string.Join(", ", invalidProductIds)}");
        }
    }
}
