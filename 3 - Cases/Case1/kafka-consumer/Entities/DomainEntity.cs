using System;
using System.Collections.Generic;
using System.Text;

namespace kafka_consumer.Entities
{
    public class DomainEntity
    {
        public DomainEntity(string value)
        {
            ValueTest = Guid.Parse(value);
        }
        public Guid ValueTest { get; private set; }
    }
}
