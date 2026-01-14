using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Application.Handlers.Customers.Commands.CreateCustomer;
using OrderProcessingSystem.Application.Handlers.Customers.Queries.GetAllCustomers;

namespace OrderProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await sender.Send(new GetAllCustomersRequest()));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCustomerRequest request, [FromServices] IValidator<CreateCustomerRequest> validator)
        {
            try
            {
                var validationResult = await validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }
                var customerId = await sender.Send(request);
                return Ok(new { Id = customerId });
            }
            catch
            {
                throw;
            }
        }
    }
}
