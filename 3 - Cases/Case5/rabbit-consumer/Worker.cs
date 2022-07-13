using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using rabbit_consumer.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace rabbit_consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRabbitMqListener _rabbitMqListener;

        public Worker(ILogger<Worker> logger, IRabbitMqListener rabbitMqListener)
        {
            _logger = logger;
            _rabbitMqListener = rabbitMqListener;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
                _rabbitMqListener.Consume(stoppingToken);
            }
        }

        public override void Dispose()
        {
            _rabbitMqListener.Dispose();
            base.Dispose();
        }
    }
}
