using System.Threading;
using System.Threading.Tasks;

namespace rabbit_consumer.Interfaces
{
    public interface IRabbitService
    {
        Task Consume(CancellationToken cancellationToken);
    }
}
