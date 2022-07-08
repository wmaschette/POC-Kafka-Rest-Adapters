using kafka_consumer.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace kafka_consumer.Interfaces
{
    public interface IDomainService
    {
        void Execute(DomainEntity value);
    }
}
