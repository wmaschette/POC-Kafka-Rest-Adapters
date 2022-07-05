using Api_Consumer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Consumer.Domain
{
    public class DomainService : IDomainService
    {
        private readonly ILogger<DomainService> _logger;

        public DomainService(ILogger<DomainService> logger)
        {
            _logger = logger;
        }
        public async Task Execute(Guid value)
        {
            await Task.Run(() => _logger.LogInformation($"Dominio executado: {value}"));
        }
    }
}
