using System;

namespace Andgasm.ServiceBus
{
    public interface IExceptionArgs
    {
        Exception Exception { get; set; }
    }
}
