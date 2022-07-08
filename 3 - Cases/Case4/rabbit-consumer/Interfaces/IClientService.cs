using System;
using System.Threading.Tasks;

namespace rabbit_consumer.Interfaces
{
    public interface IClientService
    {
        Task CallApi(Guid request);
    }
}