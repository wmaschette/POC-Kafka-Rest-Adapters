using System.Threading;
using System.Threading.Tasks;

namespace rabbit_consumer.Interfaces
{
    public interface IRabbitMqListener
    {
        void Consume(CancellationToken cancellationToken);
        void Dispose();
    }
}
