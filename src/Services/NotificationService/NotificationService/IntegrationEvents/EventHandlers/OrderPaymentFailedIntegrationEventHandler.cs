using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace NotificationService.IntegrationEvents.EventHandlers
{
    internal class OrderPaymentFailedIntegrationEventHandler : IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
    {
        private readonly ILogger<OrderPaymentFailedIntegrationEvent> Logger;

        public OrderPaymentFailedIntegrationEventHandler(ILogger<OrderPaymentFailedIntegrationEvent> logger)
        {
            Logger = logger;
        }
        public Task Handle(OrderPaymentFailedIntegrationEvent @event)
        {
            Logger.LogInformation($"Order Payment failed with OrderId: { @event.OrderId }, Error Message: { @event.ErrorMessage }");

            return Task.CompletedTask;
        }
    }
}
