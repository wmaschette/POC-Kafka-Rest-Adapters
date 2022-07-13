using Api_Consumer.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Api_Consumer
{
    public class RabbitMqWorker : BackgroundService
    {
        private readonly ILogger<RabbitMqWorker> _logger;
        private readonly IRabbitMqListener _rabbitMqListener;

        public RabbitMqWorker(ILogger<RabbitMqWorker> logger, IRabbitMqListener rabbitMqListener)
        {
            _logger = logger;
            _rabbitMqListener = rabbitMqListener;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                await _rabbitMqListener.Consume(stoppingToken);
            }
        }

        public override void Dispose()
        {
            _rabbitMqListener.Dispose();
            base.Dispose();
        }
    }
}
