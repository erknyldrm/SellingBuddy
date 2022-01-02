using BasketService.Api.Core.Application.Repository;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BasketService.Api.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IBasketRepository Repository;
        private readonly ILogger<OrderCreatedIntegrationEvent> Logger;

        public OrderCreatedIntegrationEventHandler(IBasketRepository repository, ILogger<OrderCreatedIntegrationEvent> logger)
        {
            Repository = repository;
            Logger = logger;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {
            Logger.LogInformation("Handling integration event:{IntegrationEventId} at BasketService.Api", @event.Id);

            await Repository.DeleteBasketAsync(@event.UserId.ToString());                       
        }
    }
}
