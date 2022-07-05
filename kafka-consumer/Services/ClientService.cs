using kafka_consumer.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;


namespace kafka_consumer.Services
{
    public class ClientService : IClientService
    {
        private readonly ILogger<ClientService> _logger;
        public ClientService(ILogger<ClientService> logger)
        {
            _logger = logger;
        }
        public async Task CallApi(Guid request)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:5011/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/v1/home", request);

                _logger.LogInformation($"Consuming with: {response.EnsureSuccessStatusCode().StatusCode}");
            }
        }
    }
}
