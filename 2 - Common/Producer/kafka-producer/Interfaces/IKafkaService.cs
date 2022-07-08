using kafka_producer.Models;

namespace kafka_producer.Interfaces
{
    public interface IKafkaService
    {
        void produce(ref Pedido pedido);
    }
}
