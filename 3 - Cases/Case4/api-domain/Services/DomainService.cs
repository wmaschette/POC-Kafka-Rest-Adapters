using api_domain.Entities;
using api_domain.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace api_domain.Services
{
    public class DomainService : IDomainService
    {
        private readonly ILogger<DomainService> _logger;

        public DomainService(ILogger<DomainService> logger)
        {
            _logger = logger;
        }
        public async Task Execute(DomainEntity value)
        {
            await Task.Run(() => _logger.LogInformation($"Dominio executado: {value}"));
        }
    }
}
