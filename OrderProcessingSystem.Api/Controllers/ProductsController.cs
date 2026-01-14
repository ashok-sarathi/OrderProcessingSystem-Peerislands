using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingSystem.Application.Handlers.Products.Queries.GetAllProducts;

namespace OrderProcessingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await sender.Send(new GetAllProductsRequest()));
            }
            catch
            {
                throw;
            }
        }
    }
}
