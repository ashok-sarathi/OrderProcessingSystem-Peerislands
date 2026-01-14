using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler(OrderProcessingSystemContext context) : IRequestHandler<CreateOrderRequest, Guid>
    {
        public async Task<Guid> Handle(CreateOrderRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ValidateRequest(request);

                var products = context.Products.AsNoTracking().Select(p => new { p.Id, p.Price });
                var orderDate = DateTime.UtcNow;

                var order = new Order()
                {
                    Id = Guid.NewGuid(),
                    OrderStatus = OrderStatus.PENDING,
                    OrderDate = orderDate,
                    LastModifiedDate = orderDate,
                    CustomerId = request.CustomerId.HasValue ? request.CustomerId.Value : throw new BadRequestException("Invalid CustomerId"),
                    OrderItems = request.Items.Select(item => new OrderItem
                    {
                        Id = Guid.NewGuid(),
                        ItemId = item.ItemId.HasValue ? item.ItemId.Value : throw new BadRequestException("Invalid ItemId"),
                        Quantity = item.Quantity,
                        Price = products.First(p => p.Id == item.ItemId).Price
                    }).ToList()
                };

                await context.Orders.AddAsync(order, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                return order.Id;
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
