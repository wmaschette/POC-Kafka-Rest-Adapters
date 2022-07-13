using rabbit_consumer.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace rabbit_consumer.Services
{
    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        private readonly IConfiguration _configuration;
        public ClientService(ILogger<ClientService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task CallApi(Guid request)
        {
            using (var client = new HttpClient())
            {
                ///enviar para appsettings
                client.BaseAddress = new Uri(_configuration["Api"]);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/v1/home", request);

                _logger.LogInformation($"Consuming with: {response.EnsureSuccessStatusCode().StatusCode}");
            }
        }
    }
}
