using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using OrderProcessingSystem.Data.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Application.Rules.OrderRules
{
    public class CreateOrderRule(OrderProcessingSystemContext context) : ICreateOrderRule<CreateOrderRequest, Guid>
    {
        public async Task<Guid> Apply(CreateOrderRequest request, CancellationToken cancellationToken)
        {
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
    }
}
