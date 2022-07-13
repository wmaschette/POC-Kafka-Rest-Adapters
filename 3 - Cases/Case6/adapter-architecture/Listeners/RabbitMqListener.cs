using Api_Consumer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Api_Consumer.Listeners
{
    public class RabbitMqListener : IRabbitMqListener, IDisposable
    {
        private readonly ILogger<RabbitMqListener> _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly IConfiguration _configuration;
        private readonly IDomainService _domain;

        public RabbitMqListener(ILogger<RabbitMqListener> logger, IDomainService domain, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            InitRabbitMQ();
            _domain = domain;
        }

        private void InitRabbitMQ()
        {
            var connectionFactory = new ConnectionFactory()
            {
                UserName = "guest",
                Password = "guest",
                HostName = _configuration["RabbitMq"]
            };
            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclarePassive("teste-retry");

            //_connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        public async Task Consume(CancellationToken cancellationToken)
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

        public async Task Produce(string mensage)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = false;
            var messagebuffer = Encoding.Default.GetBytes(mensage);

            _channel.BasicPublish("", "teste-retry", properties, messagebuffer);
            _logger.LogInformation("Mensagem publicada com suceeso");
        }


        private void HandleMessage(string content)
        {
            _logger.LogInformation($"---------------- RETRY -----------------");
            _logger.LogInformation($"consumer received {content}");
            _domain.Execute(Guid.Parse(content));
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
