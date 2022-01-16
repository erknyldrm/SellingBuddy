using MediatR;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.DomainEventHandlers
{
    public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly IBuyerReporsitory BuyerReporsitory;

        public OrderStartedDomainEventHandler(IBuyerReporsitory buyerReporsitory) => BuyerReporsitory = buyerReporsitory;
      
        public async Task Handle(OrderStartedDomainEvent orderStartedDomainEvent, CancellationToken cancellationToken)
        {
            var cardTypeId = (orderStartedDomainEvent.CardTypeId != 0) ? orderStartedDomainEvent.CardTypeId : 1;

            var buyer = await BuyerReporsitory.GetSingleAsync(i => i.Name == orderStartedDomainEvent.UserName);

            bool buyerOriginallyExisted = buyer != null;

            if (!buyerOriginallyExisted)
            {
                buyer = new Buyer(orderStartedDomainEvent.UserName);
            }

            buyer.VerifyOrAddPaymentMethod(cardTypeId,
                                           $"Payment method on {DateTime.UtcNow}",
                                           orderStartedDomainEvent.CardNumber,
                                           orderStartedDomainEvent.CardSecurityNumber,
                                           orderStartedDomainEvent.CardHolderName,
                                           orderStartedDomainEvent.CardExpiration,
                                           orderStartedDomainEvent.Order.Id);

            var buyerUpdated = buyerOriginallyExisted ? BuyerReporsitory.Update(buyer) : await BuyerReporsitory.AddAsync(buyer);

            await BuyerReporsitory.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}
