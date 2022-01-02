using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Core.Domain.Models;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;

namespace BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository Repository;
        private readonly IIdentityService IdentityService;
        private readonly IEventBus EventBus;
        private readonly ILogger<BasketController> Logger;

        public BasketController(IBasketRepository repository, IIdentityService identityService, IEventBus eventBus, ILogger<BasketController> logger)
        {
            Repository = repository;
            IdentityService = identityService;
            EventBus = eventBus;
            Logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Basket service is up and running.");
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string id)
        {
            var basket = await Repository.GetBasketAsync(id);

            return Ok(basket ?? new CustomerBasket(id));
        }

        [HttpPost]
        [Route("update")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> ActionResult([FromBody] CustomerBasket basket)
        {
            return Ok(await Repository.UpdateBasketAsync(basket));  
        }

        [HttpPost]
        [Route("addItem")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult> AddItemToBasket([FromBody] BasketItem basketItem)
        {
            var userId = IdentityService.GetUserName().ToString();

            var basket = await Repository.GetBasketAsync(userId);

            if (basket == null) 
                basket = new CustomerBasket(userId);

            await Repository.UpdateBasketAsync(basket);

            return Ok();
        }

        [HttpPost]
        [Route("checkout")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> CheckoutAsync([FromBody] BasketCheckout basketCheckout)
        {
            var userId = basketCheckout.Buyer;

            var basket = await Repository.GetBasketAsync(userId);

            if (basket == null)
            {
                return BadRequest();
            }

            var userName = IdentityService.GetUserName();

            var eventMessage = new OrderCreatedIntegrationEvent(userId, 
                userName, 
                basketCheckout.City, 
                basketCheckout.Street,
                basketCheckout.State,
                basketCheckout.Country,
                basketCheckout.ZipCode,
                basketCheckout.CardNumber,
                basketCheckout.CardHolderName,
                basketCheckout.CardExpiration,
                basketCheckout.CardSecuriyNumber,
                basketCheckout.CardTypeId,
                basketCheckout.Buyer,
                basket);

            try
            {
                EventBus.Publish(eventMessage);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex, "Error publising intagration event:{IntegrationEventId}", eventMessage.Id);
            }

            return Accepted();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string id)
        {
            await Repository.DeleteBasketAsync(id);
        }
    }
}
