using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasketService.Api.Infrastructure.Reporsitory
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ILogger<BasketRepository> _logger;
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public BasketRepository(ILoggerFactory loggerFactory, ConnectionMultiplexer redis)
        {
           //_logger = LoggerFactory.Create()

            _redis = redis;
            _database = _redis.GetDatabase();   
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            return await _database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            var data = await _database.StringGetAsync(customerId);

            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();

            return data?.Select(x => x.ToString());
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            var created = await _database.StringSetAsync(customerBasket.BuyerId, JsonConvert.SerializeObject(customerBasket));

            if (!created)
            {
                return null;
            }

            return await GetBasketAsync(customerBasket.BuyerId);
        }

        private IServer GetServer()
        {
            var endPoint = _redis.GetEndPoints();
            return _redis.GetServer(endPoint.First());
        }
    }
}
