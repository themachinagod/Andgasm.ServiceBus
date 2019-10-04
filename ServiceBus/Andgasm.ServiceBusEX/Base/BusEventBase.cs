
namespace Andgasm.ServiceBus
{
    public class BusEventBase : IBusEvent
    {
        public string LockToken { get; set; }
        public byte[] Body { get; set; }

        public BusEventBase(byte[] body)
        {
            Body = body;
        }
    }
}
