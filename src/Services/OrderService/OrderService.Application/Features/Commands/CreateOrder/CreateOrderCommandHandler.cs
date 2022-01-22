using EventBus.Base.Abstraction;
using MediatR;
using OrderService.Application.IntegrationEvents;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository OrderRepository;
        private readonly IEventBus EventBus;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IEventBus eventBus)
        {
            OrderRepository = orderRepository;
            EventBus = eventBus;
        }

        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var address = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);

            Order dbOrder = new(request.UserName, address, request.CardTypeId, request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration, null);

            request.OrderItems.ToList().ForEach(i => dbOrder.AddOrderItem(i.ProductId, i.ProductName, i.UnitPrice, i.PictureUrl, i.Units));

            await OrderRepository.AddAsync(dbOrder);    
            await OrderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            var orderStatedIntegrationEvent = new OrderStartedIntegrationEvent(request.UserName);

            EventBus.Publish(orderStatedIntegrationEvent);

            return true;
           
        }
    }
}
