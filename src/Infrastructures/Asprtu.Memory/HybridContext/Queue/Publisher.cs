using Asprtu.Memory.HybridContext.Contracts.Queue;

namespace Asprtu.Memory.HybridContext.Queue;

public class Publisher<T> : QueueCache<T>, IPublisher<T>
{
    public Publisher(QueueOptions options, QueueContext<T> context, ILoggerFactory loggerFactory)
      : base(options, context, loggerFactory)
    { }

    /// <summary>
    /// 向队列中添加消息
    /// </summary>
    /// <param name="message">待发布的消息</param>
    /// <returns>如果成功加入队列则返回true，否则返回false</returns>
    public bool TryEnqueue(T message)
    {
        Subject?.OnNext(message);

        try
        {
            Enqueue(message);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        finally
        {
            Signal.Release();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }
}