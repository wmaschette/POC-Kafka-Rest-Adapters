using Api_Consumer.Interfaces;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Api_Consumer.Services
{
    public class KafkaListener : IKafkaListener
    {
        private readonly ILogger<KafkaWorker> _logger;
        private readonly IConfiguration Configuration;
        private ConsumerConfig _config;
        private readonly IDomainService _domain;

        public KafkaListener(ILogger<KafkaWorker> logger, IConfiguration configuration, IDomainService domain)
        {
            _logger = logger;
            Configuration = configuration;
            _config = new ConsumerConfig
            {
                BootstrapServers = Configuration["KafkaBootstrapServers"],
                GroupId = "consumerGroupCaseD",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoOffsetStore = false
            };
            _domain = domain;
        }

        public async Task consume(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                $"Iniciando leitura do tópico: {Configuration["TopicName"]}");

            using (IConsumer<string, string> consumer = new ConsumerBuilder<string, string>(_config).Build())
            {
                consumer.Subscribe(Configuration["TopicName"]);

                int executionCount = 0;
                var watch = Stopwatch.StartNew();
                try
                {
                    while (true)
                    {
                        _logger.LogInformation($"--------------- Listener {DateTime.Now.ToLongTimeString()} --------------");
                        var resposta = consumer.Consume(cancellationToken);
                        //_logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} - " +
                        //$"Chave: {resposta.Message.Key}, " +
                        //$"Mensagem: {resposta.Message.Value}, " +
                        //$"offset: {resposta.Offset.Value}, " +
                        //$"partition: {resposta.Partition.Value}");

                        consumer.StoreOffset(resposta);

                        await _domain.Execute(Guid.Parse(resposta.Message.Value));

                        resposta = null;
                        executionCount++;

                        if (executionCount >= 100)
                        {
                            executionCount = 0;
                            watch.Stop();
                            _logger.LogInformation(watch.ElapsedMilliseconds.ToString());
                            watch.Restart();
                            //break;
                        }
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
