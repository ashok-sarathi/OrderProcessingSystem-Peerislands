
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Helper.Enums;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Api.BackgroundServices
{
    [ExcludeFromCodeCoverage]
    public class OrderProcessorBackgroundService : BackgroundService
    {
        private readonly TimeSpan _interval;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrderProcessorBackgroundService> _logger;

        public OrderProcessorBackgroundService(ILogger<OrderProcessorBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            using var scope = _scopeFactory.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            _interval = TimeSpan.FromMilliseconds(int.Parse(config["OrderProcessBackgroundServiceIntervalMs"] ?? throw new NullReferenceException()));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Order background service started.");
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var sender = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var context = scope.ServiceProvider.GetRequiredService<OrderProcessingSystemContext>();

                    var pendingOrders = context.Orders.AsNoTracking()
                        .Where(o => o.OrderStatus == OrderStatus.PENDING)
                        .ToList();

                    var processingOrders = context.Orders.AsNoTracking()
                        .Where(o => o.OrderStatus == OrderStatus.PROCESSING)
                        .ToList();

                    var shippedOrders = context.Orders.AsNoTracking()
                        .Where(o => o.OrderStatus == OrderStatus.SHIPPED)
                        .ToList();

                    foreach (var order in pendingOrders)
                    {
                        await sender.Send(new UpdateOrderStatusRequest(order.Id, OrderStatus.PROCESSING), stoppingToken);
                    }

                    foreach (var order in processingOrders)
                    {
                        await sender.Send(new UpdateOrderStatusRequest(order.Id, OrderStatus.SHIPPED), stoppingToken);
                    }

                    foreach (var order in shippedOrders)
                    {
                        await sender.Send(new UpdateOrderStatusRequest(order.Id, OrderStatus.DELIVERED), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking orders");
                }
                _logger.LogInformation("Order background service completed.");
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
