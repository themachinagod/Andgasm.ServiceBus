using Microsoft.Azure.ServiceBus;
using RawRabbit.Configuration;
using RawRabbit.Context;
using RawRabbit.vNext;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Andgasm.ServiceBus
{
    public class RabbitBusClient : BusClientBase
    {
        #region Fields
        private RawRabbit.vNext.Disposable.IBusClient _busClient;

        private Func<IBusEvent, CancellationToken, Task> _handlerCallback;
        private Func<IExceptionArgs, Task> _exceptionCallback;
        #endregion

        #region Properties
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
        public string SubscriptionName { get; set; }
        #endregion

        public RabbitBusClient(string connstr, string queue) : base(connstr, queue)
        {
        }

        #region Bus Operations
        public override void InitialiseBus()
        {
            var busConfig = new RawRabbitConfiguration
            {
                Username = "guest",
                Password = "guest",
                Port = 5672,
                VirtualHost = "/",
                PersistentDeliveryMode = true,
                Hostnames = { ConnectionString },
            };
            _busClient = BusClientFactory.CreateDefault(busConfig);
        }

        public override void RecieveEvents(Func<IExceptionArgs, Task> exceptionCallback, 
                                  Func<IBusEvent, CancellationToken, Task> handlerCallback)
        {
            _handlerCallback = handlerCallback;
            _exceptionCallback = exceptionCallback;
            _busClient.SubscribeAsync<BusEventBase>(HandleMessage, cfg => cfg
                .WithRoutingKey(QueueName));
        }

        public override async Task SendEvent(IBusEvent message)
        {
            var msg = new BusEventBase(message.Body);
            await _busClient.PublishAsync(msg, Guid.NewGuid(), cfg => cfg
                .WithRoutingKey(QueueName));
        }

        public override async Task CompleteEvent(string locktoken)
        {
            //await _busClient..CompleteAsync(locktoken);
        }

        public override async Task AbandonEvent(string locktoken)
        {
            //await _busClient..CompleteAsync(locktoken);
        }

        public override async Task Close()
        {
            await _busClient.ShutdownAsync();
        }
        #endregion

        #region Handler Wrappers
        private Task HandleMessage(BusEventBase m, MessageContext c)
        {
            return _handlerCallback(m, new CancellationToken());
        }

        private Task HandleException(ExceptionReceivedEventArgs args)
        {
            IExceptionArgs e = new ExceptionArgsBase(args.Exception);
            return _exceptionCallback(e);
        }
        #endregion
    }
}
