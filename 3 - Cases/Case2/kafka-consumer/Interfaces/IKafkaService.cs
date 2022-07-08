using System.Threading;
using System.Threading.Tasks;

namespace kafka_consumer.Interfaces
{
    public interface IKafkaService
    {
        Task consume(CancellationToken cancellationToken);
    }
}
