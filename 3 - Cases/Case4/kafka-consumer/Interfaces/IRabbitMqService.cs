using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kafka_consumer.Interfaces
{
    public interface IRabbitMqService
    {
        void PublishMessage(string mensage);
    }
}
