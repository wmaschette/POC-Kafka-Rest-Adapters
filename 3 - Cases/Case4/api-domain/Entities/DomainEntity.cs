using System;
using System.Collections.Generic;
using System.Text;

namespace api_domain.Entities
{
    public class DomainEntity
    {
        public DomainEntity(Guid value)
        {
            ValueTest = value;
        }
        public Guid ValueTest { get; private set; }
    }
}
