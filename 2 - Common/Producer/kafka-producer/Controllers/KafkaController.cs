using kafka_producer.Interfaces;
using kafka_producer.Models;
using Microsoft.AspNetCore.Mvc;

namespace kafka_producer.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class KafkaController : ControllerBase
    {
        private readonly IKafkaService _kafkaService;

        public KafkaController(IKafkaService kafkaService)
        {
            _kafkaService = kafkaService;
        }

        [HttpPost]
        public ActionResult<Pedido> PostPedido(Pedido pedido)
        {
            _kafkaService.produce(ref pedido);
            return pedido;
        }
    }
}