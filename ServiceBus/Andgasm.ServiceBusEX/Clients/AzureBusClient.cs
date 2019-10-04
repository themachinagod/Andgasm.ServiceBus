using Microsoft.Azure.ServiceBus;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Andgasm.ServiceBus
{
    public class AzureBusClient : BusClientBase
    {
        #region Fields
        private ITopicClient _topicClient;
        private ISubscriptionClient _subscriptionClient;

        private Func<IBusEvent, CancellationToken, Task> _handlerCallback;
        private Func<IExceptionArgs, Task> _exceptionCallback;
        #endregion

        #region Properties
        public string SubscriptionName { get; set; }
        #endregion

        public AzureBusClient(string connstr, string queue, string subscription) : base (connstr, queue)
        {
            SubscriptionName = subscription;
        }

        #region Bus Operations
        public override void InitialiseBus()
        {
            _topicClient = new TopicClient(ConnectionString, QueueName);
            _subscriptionClient = new SubscriptionClient(ConnectionString, QueueName, SubscriptionName);
        }

        public override void RecieveEvents(Func<IExceptionArgs, Task> exceptionCallback, 
                                  Func<IBusEvent, CancellationToken, Task> handlerCallback)
        {
            _handlerCallback = handlerCallback;
            _exceptionCallback = exceptionCallback;
            var messageHandlerOptions = new MessageHandlerOptions(HandleException)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false,
            };
            _subscriptionClient.RegisterMessageHandler(HandleMessage, messageHandlerOptions);
        }

        public override async Task SendEvent(IBusEvent message)
        {
            var msg = new Message(message.Body);
            await _topicClient.SendAsync(msg);
        }

        public override async Task AbandonEvent(string locktoken)
        {
            await _subscriptionClient.AbandonAsync(locktoken);
        }

        public override async Task CompleteEvent(string locktoken)
        {
            await _subscriptionClient.CompleteAsync(locktoken);
        }

        public override async Task Close()
        {
            await _topicClient.CloseAsync();
            await _subscriptionClient.CloseAsync();
        }
        #endregion

        #region Handler Wrappers
        private Task HandleMessage(Message m, CancellationToken c)
        {
            BusEventBase evt = new BusEventBase(m.Body);
            evt.LockToken = m.SystemProperties.LockToken;
            return _handlerCallback(evt, c);
        }

        private Task HandleException(ExceptionReceivedEventArgs args)
        {
            IExceptionArgs e = new ExceptionArgsBase(args.Exception);
            return _exceptionCallback(e);
        }
        #endregion
    }
}
