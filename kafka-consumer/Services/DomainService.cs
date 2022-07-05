using kafka_consumer.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace kafka_consumer.Services
{
    public class DomainService : IDomainService
    {
        private readonly ILogger<DomainService> _logger;

        public DomainService(ILogger<DomainService> logger)
        {
            _logger = logger;
        }
        public void Execute(Guid value)
        {
            _logger.LogInformation($"Dominio executado: {value}");
        }
    }
}
