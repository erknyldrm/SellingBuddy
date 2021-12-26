using EventBus.Base.Abstraction;
using EventBus.Base.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Api.IntegrationEvents.Events;
using System.Threading.Tasks;

namespace PaymentService.Api.IntegrationEvents.EventHandlers
{
    public class OrderStartedIntegrationEventHandler : IIntegrationEventHandler<OrderStartedIntegrationEvent>
    {
        private readonly IConfiguration Configuration;
        private readonly IEventBus EventBus;
        private readonly ILogger Logger;

        public OrderStartedIntegrationEventHandler(IConfiguration configuration, IEventBus eventBus, ILogger<OrderStartedIntegrationEventHandler> logger)
        {
            Configuration = configuration;
            EventBus = eventBus;    
            Logger = logger;
        }

        public Task Handle(OrderStartedIntegrationEvent @event)
        {
            //Fake payment process
            string keyword = "PaymentSuccess";
            bool paymentSuccessFlag = Configuration.GetValue<bool>(keyword);

            IntegrationEvent paymentEvent = paymentSuccessFlag
                ? new OrderPaymentSuccessIntegrationEvent(@event.OrderId)
                : new OrderPaymentFailedIntegrationEvent(@event.OrderId, "this is a fake error message");

            Logger.LogInformation($"OrderCreatedIntegrationEventHandler in PaymentService is fired with PaymentSuccess: {paymentSuccessFlag}, orderId:{@event.OrderId}");


          //  paymentEvent.CorrelationId = @event.CorrelationId;

        //    Log.BindProperty("CorrelationId", @event.CorrelationId, false, out LogEventProperty p);

            EventBus.Publish(paymentEvent);


            return Task.CompletedTask;
        }
    }
}
