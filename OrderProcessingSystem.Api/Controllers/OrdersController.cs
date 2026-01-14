using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.UpdateOrderStatus;
using OrderProcessingSystem.Application.Handlers.Orders.Commands.CreateOrder;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetAllOrders;
using OrderProcessingSystem.Application.Handlers.Orders.Queries.GetOrderById;
using OrderProcessingSystem.Data.Helper.Enums;
using System.Threading.Tasks;

namespace OrderProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] OrderStatus? orderStatus, [FromQuery] Guid? customerId)
        {
            return Ok(await sender.Send(new GetAllOrdersRequest(orderStatus, customerId)));
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetById(Guid orderId)
        {
            return Ok(await sender.Send(new GetOrderByIdRequest(orderId)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrderRequest request, [FromServices] IValidator<CreateOrderRequest> validator)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var orderId = await sender.Send(request);
                return CreatedAtAction(nameof(GetById), new { orderId }, null);
            }
            catch
            {
                throw;
            }
        }

        [HttpPatch("[action]/{orderId}")]
        public async Task<IActionResult> CancelOrder([FromRoute] Guid orderId)
        {
            try
            {
                await sender.Send(new UpdateOrderStatusRequest(orderId, OrderStatus.CANCELED));
                return NoContent();
            }
            catch
            {
                throw;
            }
        }
    }
}
