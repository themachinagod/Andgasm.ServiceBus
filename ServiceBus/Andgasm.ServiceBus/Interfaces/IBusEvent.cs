
namespace Andgasm.ServiceBus
{
    public interface IBusEvent
    {
        string LockToken { get; set; }
        byte[] Body { get; set; }
    }
}
