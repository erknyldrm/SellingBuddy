using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    internal class OrderPaymentSuccessIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentSuccessIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentSuccessIntegrationEvent> Logger;

        public OrderPaymentSuccessIntegrationEventHandler(ILogger<OrderPaymentSuccessIntegrationEvent> logger)
        {
            Logger = logger;
        }
        public Task Handle(OrderPaymentSuccessIntegrationEvent @event)
        {
            Logger.LogInformation($"Order Payment success with OrderId: { @event.OrderId }");

            return Task.CompletedTask;
        }
    }
}
