using EventBus.Base.Abstraction;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Application.Features.Commands.CreateOrder;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MediatR;
using Microsoft.Extensions.Logging;

namespace OrderService.Api.IntegrationEvents.EventHandlers
{
    public class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        private readonly IMediator mediator;
        private readonly ILogger<OrderCreatedIntegrationEventHandler> logger;

        public OrderCreatedIntegrationEventHandler(IMediator mediator, ILogger<OrderCreatedIntegrationEventHandler> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public async Task Handle(OrderCreatedIntegrationEvent @event)
        {

            logger.LogInformation("Handling Intagration Event: {IntagrationEventId} at {AppName}", @event.Id, typeof(Startup).Namespace);

            var items = (from p in @event.Basket.Items
                         select new BasketService.Api.Core.Domain.Models.BasketItem
                         {
                             Id = p.Id,
                             OldUnitPrice = p.OldUnitPrice,
                             Quantity = p.Quantity,
                             UnitPrice = p.UnitPrice,
                             PictureUrl = p.PictureUrl,
                             ProductId = p.ProductId,
                             ProductName = p.ProductName
                         }).ToList();


            var createOrderCommand = new CreateOrderCommand(items, @event.UserId, @event.UserName, @event.Street,
                @event.State, @event.Country, @event.ZipCode, @event.CardNumber, @event.CardHolderName, @event.CardSecurityNumber,
                @event.CardExpiration, @event.CardTypeId);


            await mediator.Send(createOrderCommand);
        }
    }
}
