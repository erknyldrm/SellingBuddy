using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBus
    {
        abstract void Publish(IntegrationEvent @event);
        abstract void Subscribe<T, TH>() where T: IntegrationEvent where TH: IIntegrationEventHandler<T>;
        abstract void UnSubscribe<T, TH>() where T: IntegrationEvent where TH: IIntegrationEventHandler<T>;
    }
}
