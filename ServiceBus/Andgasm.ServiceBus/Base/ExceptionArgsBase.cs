using System;

namespace Andgasm.ServiceBus
{
    public class ExceptionArgsBase : IExceptionArgs
    {
        public Exception Exception { get; set; }

        public ExceptionArgsBase(Exception ex)
        {
            Exception = ex;
        }
    }
}
