using MediatR;
using OrderProcessingSystem.Application.Dtos.Orders;
using OrderProcessingSystem.Application.Helper.Exceptions;
using OrderProcessingSystem.Application.Rules.OrderRules.IOrderRules;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandler(IGetOrderByIdRule<GetOrderByIdRequest, Order?> getOrderByIdRule) : IRequestHandler<GetOrderByIdRequest, OrderDetailsDto>
    {
        public async Task<OrderDetailsDto> Handle(GetOrderByIdRequest request, CancellationToken cancellationToken)
        {
            var order = await getOrderByIdRule.Apply(request, cancellationToken);
            if (order == null)
                throw new NotFoundException($"Given order id not found : {request.OrderId}");

            return MapOrderDto(order);
        }

        private OrderDetailsDto MapOrderDto(Order order)
        {
            var orderDetailsDto = new OrderDetailsDto();

            orderDetailsDto.OrderId = order.Id;
            orderDetailsDto.OrderStatus = order.OrderStatus.ToString();
            orderDetailsDto.OrderDate = order.OrderDate;
            orderDetailsDto.LastModifiedDate = order.LastModifiedDate;
            orderDetailsDto.CustomerId = order.CustomerId;
            orderDetailsDto.CustomerName = order.Customer.Name;
            orderDetailsDto.CustomerEmail = order.Customer.Email;
            orderDetailsDto.CustomerPhone = order.Customer.Phone;
            orderDetailsDto.ShippingAddress = order.Customer.ShippingAddress;

            orderDetailsDto.Items = order.OrderItems.Select(oi => new OrderDetailsItemDto
            {
                ItemId = oi.ItemId,
                ProductName = oi.Item.Name,
                ProductDescription = oi.Item.Description,
                Quantity = oi.Quantity,
                Price = oi.Price
            }).ToList();

            orderDetailsDto.TotalAmount = orderDetailsDto.Items.Sum(i => i.Price * i.Quantity);

            return orderDetailsDto;
        }
    }
}
