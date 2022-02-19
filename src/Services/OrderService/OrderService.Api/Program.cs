using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderService.Api.Extensions;
using OrderService.Infrastructure.Context;
using System.IO;

namespace OrderService.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(GetConfiguration(), args);

            host.MigrateDbContext<OrderDbContext>((context, services) =>
            {
                var logger = services.GetService<ILogger<OrderDbContext>>();

                var dbContextSeeder = new OrderDbContextSeed();
                dbContextSeeder.SeedAsync(context, logger).Wait();

            });
            //CreateHostBuilder(args).Build().Run();
        }

        static IWebHost BuildWebHost(IConfiguration configuration , string[] args) =>
            WebHost.CreateDefaultBuilder(args)  
           .UseDefaultServiceProvider((context, options) =>
           {
               options.ValidateOnBuild = false;
               options.ValidateScopes = false;
           })
            .ConfigureAppConfiguration(p=>p.AddConfiguration(configuration))
            .UseStartup<Startup>()
            .Build();

        static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }
       
    }

}
