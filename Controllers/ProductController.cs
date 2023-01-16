using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShoppingService2.RabbitMQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingService2.Controllers
{
    [ApiController]
    [Route("product")]
    public class ProductController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly IRabitMQProducer _productPublisher;
        private readonly ILogger<ProductController> _logger;

        public ProductController(ILogger<ProductController> logger, IRabitMQProducer productPublisher)
        {
            _logger = logger;
            _productPublisher = productPublisher;
        }

        [HttpPost]
        [Route("")]
        public IActionResult PostProduct(Models.Product product)
        {
            _logger.LogInformation("Insert data successfully");
            var productTemp = new Models.Product {
                Name = product.Name,
                Number=product.Number,
                Price=product.Price
            };
            _logger.LogInformation("Start send data");
            _productPublisher.SendProductMessage<Models.Product>(product);
            return Ok();
        }
    }
}
