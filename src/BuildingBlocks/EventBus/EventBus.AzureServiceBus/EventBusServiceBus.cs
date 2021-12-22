using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {

        private ITopicClient topicClient;
        private ManagementClient managementClient;
        private ILogger logger;

        public EventBusServiceBus(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>;

            managementClient = new ManagementClient(config.EventBusConnectionString);

            topicClient = CreateTopicClient();
        }

        private ITopicClient CreateTopicClient()
        {
            if (topicClient != null || topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, RetryPolicy.Default);
            }

            //ensure that already exisc
            if (!managementClient.TopicExistsAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
                managementClient.CreateTopicAsync(EventBusConfig.DefaultTopicName).GetAwaiter().GetResult();


            return topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name; //example: OrderCreatedIntegrationEvent

            eventName = ProcessEventName(eventName); // exeample: OrderCreated

            var eventStr = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(@eventStr);


            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = body,
                Label = eventName
            };


            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {

            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionForEvent(eventName))
            {
                var subscriptionClient = CreateSubscriptionClientIfNotExist(eventName);

                RegisterSubscriptionClientMessageHandler(subscriptionClient);
            }

            logger.LogInformation("Subscribing to event {eventName} with {EventHandler]", eventName, typeof(TH).Name);

            SubsManager.AddSubscription<T, TH>();
        }

        public override void UnSubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;

            try
            {
                //Subscription will be there but we dont subscribe
                var subscriptionClient = CreateSubscriptionClient(eventName);

                subscriptionClient.RemoveRuleAsync(eventName).GetAwaiter().GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                logger.LogInformation("The messaging entity {eventName} could not be found", eventName);
            }

            logger.LogInformation("Unsubscribing form event {EventName}", eventName);

            SubsManager.RemoveSubscription<T, TH>();
        }

        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    //Complete the message so that it is not received again 
                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10, AutoComplete = false });
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            logger.LogError(ex, "ERROR handling message: {ExceptionMessage} -  Context: {@ExceptionContext}", ex.Message, context);

            return Task.CompletedTask;
        }

        private ISubscriptionClient CreateSubscriptionClientIfNotExist(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);

            var exist = managementClient.SubscriptionExistsAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

            if (!exist)
            {
                managementClient.CreateSubscriptionAsync(EventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
                RemoveDefaultRule(subClient);
            }

            CreateRuleIfNotExist(ProcessEventName(eventName), subClient);

            return subClient;
        }

        private void CreateRuleIfNotExist(string eventName, ISubscriptionClient subscriptionClient)
        {
            bool ruleExist;

            try
            {
                var rule = managementClient.GetRuleAsync(EventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
                ruleExist = rule != null;

            }
            catch (MessagingEntityNotFoundException)
            {
                //Azure management client doesnt have RuleExists method
                ruleExist = false;
            }

            if (!ruleExist)
            {
                subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter(),
                    Name = eventName,
                }).GetAwaiter().GetResult();
            }
        }

        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient.RemoveRuleAsync(RuleDescription.DefaultRuleName).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger.LogWarning("The messaging entity {DefaultRuleName} could not be found", RuleDescription.DefaultRuleName);
            }
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
        {
            return new SubscriptionClient(EventBusConfig.EventBusConnectionString, EventBusConfig.DefaultTopicName, GetSubName(eventName));
        }

        public override void Dispose()
        {
            base.Dispose();

            topicClient.CloseAsync().GetAwaiter().GetResult();
            managementClient.CloseAsync().GetAwaiter().GetResult();
            topicClient = null;
            managementClient = null;
        }
    }
}
