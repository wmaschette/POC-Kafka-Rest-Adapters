using Microsoft.Extensions.Configuration;
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

namespace rabbit_consumer.Services
{
    public class RabbitService : IRabbitService, IDisposable
    {
        private readonly ILogger<RabbitService> _logger;
        private readonly IClientService _clientService;
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;
        private const string QueueName = "teste";

        public RabbitService(ILogger<RabbitService> logger, IConfiguration configuration, IClientService clientService)
        {
            _logger = logger;
            _clientService = clientService;
            _configuration = configuration;

            var connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = configuration["RabbitMq"]
            };
            _connection = connectionFactory.CreateConnection();

            _channel = _connection.CreateModel();

            _channel.QueueDeclarePassive(queue: "teste2");
        }

        public async Task Consume(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += async (bc, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                _logger.LogInformation($"Processing msg: '{message}'.");
                try
                {
                    var value = JsonSerializer.Deserialize<string>(message);
                    await _clientService.CallApi(Guid.Parse(value));
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (JsonException)
                {
                    _logger.LogError($"JSON Parse Error: '{message}'.");
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
                catch (AlreadyClosedException)
                {
                    _logger.LogInformation("RabbitMQ is closed!");
                    _connection.Close();
                }
                catch (Exception e)
                {
                    _logger.LogError(default, e, e.Message);
                    _connection.Close();
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
