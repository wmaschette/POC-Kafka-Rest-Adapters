using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using rabbit_consumer.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace rabbit_consumer.Listeners
{
    public class RabbitMqListener : IRabbitMqListener, IDisposable
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private IModel _channel;
        private readonly IConfiguration _configuration;
        private readonly IClientService _clientService;

        public RabbitMqListener(ILogger<RabbitMqListener> logger, IConfiguration configuration, IClientService clientService)
        {
            _logger = logger;
            _configuration = configuration;
            _clientService = clientService;
            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            var connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = _configuration["RabbitMq"]
            };
            var connection = connectionFactory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclarePassive("teste-retry");

            //_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        public void Consume(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                _channel.BasicAck(ea.DeliveryTag, false);
                HandleMessage(message);
            };

            //consumer.Shutdown += OnConsumerShutdown;
            //consumer.Registered += OnConsumerRegistered;
            //consumer.Unregistered += OnConsumerUnregistered;
            //consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("teste-retry", false, consumer);
        }


        private void HandleMessage(string content)
        {
            _logger.LogInformation($"consumer received {content}");
            _clientService.CallApi(Guid.Parse(content));
        }

        public void Dispose()
        {
            _channel.Close();
        }
        //private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        //private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        //private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        //private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        //private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }
    }
}
