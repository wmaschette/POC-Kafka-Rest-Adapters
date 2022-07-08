using Api_Consumer.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Api_Consumer
{
    public class KafkaWorker : BackgroundService
    {
        private readonly ILogger<KafkaWorker> _logger;
        private readonly IKafkaListener _kafkaListener;

        public KafkaWorker(ILogger<KafkaWorker> logger, IKafkaListener kafkaListener)
        {
            _logger = logger;
            _kafkaListener = kafkaListener;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(500, stoppingToken);
                await _kafkaListener.consume(stoppingToken);
            }
        }
    }
}
