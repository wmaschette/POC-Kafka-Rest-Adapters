using System.Threading;

namespace kafka_consumer.Interfaces
{
    public interface IKafkaService
    {
        void consume(CancellationToken cancellationToken);
    }
}
