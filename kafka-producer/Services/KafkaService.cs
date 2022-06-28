using Confluent.Kafka;
using kafka_producer.Interfaces;
using kafka_producer.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace kafka_producer.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly ILogger<KafkaService> _logger;
        private readonly IConfiguration Configuration;
        private ProducerConfig _config;

        public KafkaService(ILogger<KafkaService> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            _config = new ProducerConfig
            {
                BootstrapServers = Configuration["KafkaBootstrapServers"],
                ClientId = Dns.GetHostName()
            };
        }

        public void produce(ref Pedido pedido)
        {
            using (var producer = new ProducerBuilder<string, string>(_config).Build())
            {
                for (int indice = 1; indice <= pedido.Mensagens; indice++)
                {
                    var resposta = producer.ProduceAsync(Configuration["TopicName"], new Message<string, string>
                    { Key = Guid.NewGuid().ToString(), Value = Guid.NewGuid().ToString() });

                    resposta.ContinueWith(task => {
                        if (task.IsFaulted)
                            _logger.LogError($"{DateTime.Now.ToString("yyyy - MM - dd HH: mm:ss.fff")} - " +
                                $"Erro ao produzir mensagem.");
                        else
                            _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                                $"Chave: {task.Result.Message.Key}, " + 
                                $"Mensagem: {task.Result.Message.Value}, " + 
                                $"offset: {task.Result.Offset}, " +
                                $"partition: {task.Result.Partition}");
                    });
                }

                producer.Flush();
            }
        }
    }
}