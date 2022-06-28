using Confluent.Kafka;
using kafka_consumer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace kafka_consumer.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration Configuration;
        private ConsumerConfig _config;

        public KafkaService(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
            _config = new ConsumerConfig
            {
                BootstrapServers = Configuration["KafkaBootstrapServers"],
                GroupId = "poc-kafka-dotnet",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoOffsetStore = false
            };
        }

        public void consume(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                $"Iniciando leitura do tópico: {Configuration["TopicName"]}");

            short mensagensParaLer = 5;

            using (IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(_config).Build())
            {
                consumer.Subscribe(Configuration["TopicName"]);
                ConsumeResult<string, string> resposta = null;

                try
                {
                    while (true)
                    {
                        resposta = consumer.Consume(cancellationToken);
                        _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                        $"Chave: {resposta.Message.Key}, " +
                        $"Mensagem: {resposta.Message.Value}, " +
                        $"offset: {resposta.Offset.Value}, " +
                        $"partition: {resposta.Partition.Value}");

                        mensagensParaLer--;

                        if (mensagensParaLer > 0)
                            consumer.StoreOffset(resposta);
                        else
                            throw new Exception("Parada proposital");

                        _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                            $"Guardado no offset para commit futuro.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                        $"Exceção no processamento, mensagem: {ex.Message}");
                }
                finally
                {
                    consumer.Close();
                }
            }
        }
    }
}
