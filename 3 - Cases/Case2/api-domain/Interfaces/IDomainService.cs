using api_domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace api_domain.Interfaces
{
    public interface IDomainService
    {
        Task Execute(DomainEntity value);
    }
}
