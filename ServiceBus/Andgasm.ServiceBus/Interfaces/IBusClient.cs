using System;
using System.Threading;
using System.Threading.Tasks;

namespace Andgasm.ServiceBus
{
    public interface IBusClient
    {
        string ConnectionString { get; set; }
        string QueueName { get; set; }

        void InitialiseBus();
        Task SendEvent(IBusEvent message);
        Task CompleteEvent(string locktoken);
        Task AbandonEvent(string locktoken);
        void RecieveEvents(Func<IExceptionArgs, Task> exceptionCallback,
                           Func<IBusEvent, CancellationToken, Task> handlerCallback);
        Task Close();
    }
}
