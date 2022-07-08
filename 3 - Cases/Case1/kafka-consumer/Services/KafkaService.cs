using Confluent.Kafka;
using kafka_consumer.Entities;
using kafka_consumer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;

namespace kafka_consumer.Services
{
    public class KafkaService : IKafkaService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration Configuration;
        private readonly IDomainService _domainService;
        private ConsumerConfig _config;

        public KafkaService(ILogger<Worker> logger, IConfiguration configuration, IDomainService domainService)
        {
            _logger = logger;
            Configuration = configuration;
            _config = new ConsumerConfig
            {
                BootstrapServers = Configuration["KafkaBootstrapServers"],
                GroupId = "consumerGroupCaseA",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoOffsetStore = false
            };
            _domainService = domainService;
        }

        public void consume(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                $"Iniciando leitura do tópico: {Configuration["TopicName"]}");

            using (IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(_config).Build())
            {
                consumer.Subscribe(Configuration["TopicName"]);
                ConsumeResult<string, string> resposta = null;

                int executionCount = 0;
                var watch = Stopwatch.StartNew();
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
                        consumer.StoreOffset(resposta);
                        executionCount++;

                        _domainService.Execute(new DomainEntity(resposta.Message.Value));

                        //if (executionCount >= 100)
                        //{
                        //    watch.Stop();
                        //    var elapsedMs = watch.ElapsedMilliseconds;
                        //    _logger.LogInformation(elapsedMs.ToString());
                        //    break;
                        //}
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
