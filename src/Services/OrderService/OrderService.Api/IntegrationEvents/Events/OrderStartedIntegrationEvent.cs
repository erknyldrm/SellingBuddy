using EventBus.Base.Events;

namespace OrderService.Api.IntegrationEvents.Events
{
    public class OrderStartedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; private set; }
        public int OrderId { get; private set; }

        public OrderStartedIntegrationEvent(string userId, int orderId)
        {
            UserId = userId;
            OrderId = orderId;
        }
    }
}
