using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Data.SqlClient;


namespace OrderService.Api.Extensions
{
    public static class HostExtension
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating db associated with context { typeof(TContext).Name } ");

                    var rety = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(8),
                        });

                    rety.Execute(() => InvokeSeeder(seeder, context, services));

                    logger.LogInformation($"Migrated db with context { typeof(TContext).Name } ");

                }
                catch (Exception ex)
                {
                    logger.LogError($"An error occoured migration the database used on context { typeof(TContext).Name }");
                }

                return host;
            }
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services)
        where TContext: DbContext
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();

            seeder(context, services);
        }    
    }
}
