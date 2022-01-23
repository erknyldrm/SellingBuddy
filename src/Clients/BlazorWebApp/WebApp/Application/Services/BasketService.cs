using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using WebApp.Application.Services.Dtos;
using WebApp.Application.Services.Interfaces;
using WebApp.Domain.Models.ViewModels;
using WebApp.Extensions;

namespace WebApp.Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient apiClient;
        private readonly IIdentityService identityService;
        private readonly ILogger<BasketService> logger;

        public BasketService(HttpClient apiClient, IIdentityService identityService, ILogger<BasketService> logger)
        {
            this.apiClient = apiClient;
            this.identityService = identityService;
            this.logger = logger;
        }

        public async Task AddItemToBasket(int productId)
        {
            var model = new
            {
                CatalogItemId = productId,
                Quantity = 1,
                BasketId = identityService.GetUserName()
            };

            //await apiClient.PostAsync("basket/items", model);
            await apiClient.PostAsync("basket/items", null);
        }

        public Task Checkout(BasketDTO basket)
        {
           // return apiClient.PostAsync("basket/checkout", basket);
            return apiClient.PostAsync("basket/checkout", null);
        }

        public async Task<Basket> GetBasket()
        {
            var response = await apiClient.GetResponseAsync<Basket>("basket" + identityService.GetUserName());

            return response ?? new Basket { BuyerId = identityService.GetUserName() };
        }

        public async Task<Basket> UpdateBasket(Basket basket)
        {
            return await apiClient.PostGetResponseAsync<Basket, Basket>("basket/update", basket);
        }
    }
}
