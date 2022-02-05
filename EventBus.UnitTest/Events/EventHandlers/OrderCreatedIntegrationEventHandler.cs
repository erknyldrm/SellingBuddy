using EventBus.Base.Abstraction;
using EventBus.UnitTest.Events.Events;
using System;
using System.Threading.Tasks;

namespace EventBus.UnitTest.Events.EventHandlers
{
    internal class OrderCreatedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
    {
        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            Console.WriteLine("Handle method worked with id: " + @event.Id);
            return Task.CompletedTask;
        }
    }
}
