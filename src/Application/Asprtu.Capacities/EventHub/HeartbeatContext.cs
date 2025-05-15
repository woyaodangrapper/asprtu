using Asprtu.Core.Protocol;
using Asprtu.Repository.MemoryCache;

namespace Asprtu.Capacities.EventHub;

public class HeartbeatContext(HeartHandles HeartHandles, QueueOptions Option, ISubscriber Subscriber)
{
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    public HeartHandles HeartHandles { get; set; } = HeartHandles;
    public QueueOptions Option { get; set; } = Option;
    public ISubscriber Subscriber { get; set; } = Subscriber;
}