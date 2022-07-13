
namespace Api_Consumer.Interfaces
{
    public interface IRabbitMqService
    {
        void PublishMessage(string mensage);
    }
}
