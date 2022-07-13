using System.Threading;
using System.Threading.Tasks;

namespace Api_Consumer.Interfaces
{
    public interface IKafkaListener
    {
        Task consume(CancellationToken cancellationToken);
    }
}
