using System.Threading;
using System.Threading.Tasks;

namespace Api_Consumer.Interfaces
{
    public interface IRabbitMqListener
    {
        Task Consume(CancellationToken cancellationToken);
        Task Produce(string mensage);

        void Dispose();
    }
}
