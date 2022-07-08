using System;
using System.Threading.Tasks;

namespace kafka_consumer.Interfaces
{
    public interface IClientService
    {
        Task CallApi(Guid request);
    }
}