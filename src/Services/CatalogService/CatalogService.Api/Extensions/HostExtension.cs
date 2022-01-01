using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using System;

namespace CatalogService.Api.Extensions
{
    public static class HostExtension
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> seeder)
            where TContext : DbContext
        {
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();  

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation("Migratiın database associated with context {DbContextName}", typeof(TContext).Name);

                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(new TimeSpan[]
                        {
                            TimeSpan.FromSeconds(3),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(7),
                        });

                 //   retry.Execute(() => InvokeSeeder(seeder, context, services)); // there is no db on this pc

                    logger.LogInformation("Migratiın database associated with context {DbContextName}", typeof(TContext).Name);

                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occured while migrations the databse used on context {DbContextName}", typeof(TContext).Name);
                }
            }

            return host;
        }

        private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) 
            where TContext : DbContext
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();

            seeder(context, services);
        }
    }
}
