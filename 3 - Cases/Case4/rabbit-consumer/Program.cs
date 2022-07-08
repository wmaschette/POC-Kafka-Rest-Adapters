using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rabbit_consumer.Interfaces;
using rabbit_consumer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rabbit_consumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IClientService, ClientService>();
                    services.AddTransient<IRabbitService, RabbitService>();
                    services.AddHostedService<Worker>();
                });
    }
}
