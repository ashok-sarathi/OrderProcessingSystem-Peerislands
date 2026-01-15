using MediatR;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Application.Rules.ProductRules.IProductRules;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler(ICreateOrderRule<CreateOrderRequest, Guid> createOrderRule, IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>> getAllProductsRule, IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>> getAllCustomersRule) : IRequestHandler<CreateOrderRequest, Guid>
    {
        public async Task<Guid> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await ValidateRequest(request, cancellationToken);

                return await createOrderRule.Apply(request, cancellationToken);
            }
            catch
            {
                throw;
            }
        }

        private async Task ValidateRequest(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            var customers = await getAllCustomersRule.Apply(new GetAllCustomersRequest(), cancellationToken);
            var allProducts = await getAllProductsRule.Apply(new GetAllProductsRequest(), cancellationToken);

            if (!customers.Any(c => c.Id == request.CustomerId))
                throw new BadRequestException($"Invalid customer ID: {request.CustomerId}");

            var products = allProducts.Select(p => p.Id);
            var invalidProductIds = request.Items
                .Where(item => item.ItemId.HasValue && !products.Contains(item.ItemId.Value))
                .Select(item => item.ItemId)
                .ToList();
            if (invalidProductIds.Any())
                throw new BadRequestException($"Invalid product IDs: {string.Join(", ", invalidProductIds)}");
        }
    }
}
