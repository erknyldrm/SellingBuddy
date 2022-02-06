using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;
using System.Collections;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using System.IO;
using System.Collections.Generic;
using OrderService.Domain.SeedWork;
using OrderService.Domain.AggregateModels.OrderAggregate;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure.Context
{
    public class OrderDbContextSeed
    {
        public async Task SeedAsync(OrderDbContext context, ILogger<OrderDbContext> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomazationData = false;
                var contentRootPath = "Seeding/Setup";


                using (context)
                {
                    context.Database.Migrate();

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomazationData
                            ? GetCardTypesFromFile(contentRootPath, logger)
                            : GetPredefinedCardTpes());

                        await context.SaveChangesAsync();
                    }
                }
            });
        }

        private IEnumerable<CardType> GetPredefinedCardTpes()
        {
            return null;
        }

        private IEnumerable<CardType> GetCardTypesFromFile(string contentRootPath, ILogger<OrderDbContext> logger)
        {
            string fileName = "CardTypes.txt";

            if (!File.Exists(fileName))
            {
                return GetPredefinedCardTypes();
            }

            var fileContent = File.ReadAllText(fileName);

            int id = 1;
            var list = fileContent.Select(i => new OrderStatus(id++, i)).Where(i => i != null);
            return list;    
        }

        private IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return Enumeration.GetAll<CardType>();
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<OrderDbContext> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => System.TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning("Exception message");
                });
        }
    }
}
