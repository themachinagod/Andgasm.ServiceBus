using System;
using System.Threading;
using System.Threading.Tasks;

namespace Andgasm.ServiceBus
{
    public abstract class BusClientBase : IBusClient
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }

        public BusClientBase(string connstr, string queuename)
        {
            ConnectionString = connstr;
            QueueName = queuename;
        }

        public abstract Task Close();

        public abstract Task AbandonEvent(string locktoken);

        public abstract Task CompleteEvent(string locktoken);

        public abstract void InitialiseBus();

        public abstract void RecieveEvents(Func<IExceptionArgs, Task> exceptionCallback, Func<IBusEvent, CancellationToken, Task> handlerCallback);

        public abstract Task SendEvent(IBusEvent message);
    }
}
