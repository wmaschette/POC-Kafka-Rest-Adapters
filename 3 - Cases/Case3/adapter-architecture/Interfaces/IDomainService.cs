using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Consumer.Interfaces
{
    public interface IDomainService
    {
        Task Execute(Guid value);
    }
}
