using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;
using OrderProcessingSystem.Application.Rules.CustomerRules;
using OrderProcessingSystem.Application.Rules.CustomerRules.ICustomerRules;
using OrderProcessingSystem.Application.Rules.OrderRules;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Application.Rules.ProductRules;
using OrderProcessingSystem.Application.Rules.ProductRules.IProductRules;
using OrderProcessingSystem.Application.Validators.Customers;
using OrderProcessingSystem.Data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Application.Helper.ServiceRegistries
{
    [ExcludeFromCodeCoverage]
    public static class ApplicationServiceRegistry
    {
        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            // Register MediatR CQRS Handlers
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(ApplicationServiceRegistry).Assembly);
            });

            // Register Validators
            services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();

            // Register Rules
            services.AddTransient<IUpdateOrderRule<UpdateOrderStatusRequest>, UpdateStatusRule>();
            services.AddTransient<ICancelOrderRule<UpdateOrderStatusRequest>, CancelOrderRule>();
            services.AddTransient<ICreateOrderRule<CreateOrderRequest, Guid>, CreateOrderRule>();
            services.AddTransient<IGetAllOrdersRule<GetAllOrdersRequest, IQueryable<Order>>, GetAllOrdersRule>();
            services.AddTransient<IGetOrderByIdRule<GetOrderByIdRequest, Order?>, GetOrderByIdRule>();
            services.AddTransient<IGetAllProductsRule<GetAllProductsRequest, IQueryable<Product>>, GetAllProductsRule>();
            services.AddTransient<ICreateCustomerRule<CreateCustomerRequest, Guid>, CreateCustomerRule>();
            services.AddTransient<IGetAllCustomersRule<GetAllCustomersRequest, IQueryable<Customer>>, GetAllCustomersRule>();
        }
    }
}
