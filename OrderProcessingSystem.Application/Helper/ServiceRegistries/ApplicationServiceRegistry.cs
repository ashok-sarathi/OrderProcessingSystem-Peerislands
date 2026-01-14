using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Rules.OrderRules;
using OrderProcessingSystem.Application.Rules.OrderRules.IRules;
using OrderProcessingSystem.Application.Validators.Customers;

namespace OrderProcessingSystem.Application.Helper.ServiceRegistries
{
    public static class ApplicationServiceRegistry
    {
        public static void RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(typeof(ApplicationServiceRegistry).Assembly);
            });

            services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();

            services.AddTransient<IUpdateOrderRule<UpdateOrderStatusRequest>, UpdateStatusRule>();
            services.AddTransient<ICancelOrderRule<UpdateOrderStatusRequest>, CancelOrderRule>();
        }
    }
}
