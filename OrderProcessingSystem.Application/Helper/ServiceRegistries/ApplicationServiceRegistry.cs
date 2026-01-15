using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Rules.OrderRules;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Application.Validators.Customers;
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
        }
    }
}
