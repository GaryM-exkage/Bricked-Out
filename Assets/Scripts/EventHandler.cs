using Easy.MessageHub;

public class EventHandler : Singleton<EventHandler>
{
    public IMessageHub hub = new MessageHub();

    protected EventHandler() {}
}
