using kafka_consumer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kafka_consumer.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly ILogger<RabbitMqService> _logger;
        private readonly IConfiguration _configuration;
        public RabbitMqService(ILogger<RabbitMqService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void PublishMessage(string mensage)
        {
            var connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = _configuration["RabbitMq"]
            };

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclarePassive("teste");
            var properties = channel.CreateBasicProperties();
            properties.Persistent = false;
            var messagebuffer = Encoding.Default.GetBytes(mensage);

            channel.BasicPublish("", "teste", properties, messagebuffer);
            _logger.LogInformation("Mensagem publicada com suceeso");
        }
    }
}
