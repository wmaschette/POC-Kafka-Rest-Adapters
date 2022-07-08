using Api_Consumer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Consumer.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDomainService _domain;

        public HomeController(ILogger<HomeController> logger, IDomainService domain)
        {
            _logger = logger;
            _domain = domain;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Post([FromBody] Guid value)
        {
            _logger.LogInformation($"--------------- API {DateTime.Now.ToLongTimeString()} --------------");
            _logger.LogInformation($"Value receipt: {value}");

            await _domain.Execute(value);

            return value;
        }

        [HttpGet]
        public string Get()
        {
            return "Teste";
        }
    }
}
