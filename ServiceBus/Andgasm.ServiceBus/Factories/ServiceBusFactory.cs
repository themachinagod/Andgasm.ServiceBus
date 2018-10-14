using System;
using System.Collections.Generic;
using System.Text;

namespace Andgasm.ServiceBus
{
    public enum BusHost
    {
        Azure,
        RabbitMQ
    }

    public class ServiceBusFactory
    {
        public static IBusClient GetBus(BusHost host, string connString, string queueName, string subscriptionName = null)
        {
            IBusClient b = null;
            switch (host)
            {
                case BusHost.Azure:
                    b =  new AzureBusClient(connString, queueName, subscriptionName);
                    break;
                case BusHost.RabbitMQ:
                    b = new RabbitBusClient(connString, queueName);
                    break;
                default:
                    throw new NotSupportedException($"Specified bus host { host.ToString() } is not supported!"); 
            }
            b.InitialiseBus();
            return b;
        }
    }
}
