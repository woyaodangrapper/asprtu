namespace Asprtu.Memory.HybridContext.Contracts.Queue;

public interface ISubscriber<T> : IDisposable
{
    /// <summary>
    /// 尝试从队列中同步阻塞地获取一条消息。
    /// </summary>
    /// <param name="message">
    /// 方法返回时，若操作成功则包含取出的消息；否则为类型 <typeparamref name="T"/> 的默认值。
    /// </param>
    /// <param name="cancellationToken">
    /// 用于观察取消请求的 <see cref="CancellationToken"/>，若在等待期间触发，则方法将抛出 <see cref="OperationCanceledException"/>。
    /// </param>
    /// <returns>
    /// 如果成功获取到消息，返回 <see langword="true"/>；否则返回 <see langword="false"/>（队列为空或已取消）。
    /// </returns>
    bool TryDequeue(out T? message, CancellationToken cancellationToken);

    /// <summary>
    /// 尝试从队列中非阻塞地获取一条消息，使用默认的取消令牌（不支持取消）。
    /// </summary>
    /// <param name="message">
    /// 方法返回时，若操作成功则包含取出的消息；否则为类型 <typeparamref name="T"/> 的默认值。
    /// </param>
    /// <returns>
    /// 如果成功获取到消息，返回 <see langword="true"/>；否则返回 <see langword="false"/>（队列为空）。
    /// </returns>
    bool TryDequeue(out T? message);

    /// <summary>
    /// 异步尝试从队列中获取一条消息，等待直到有可用项或取消。
    /// </summary>
    /// <param name="cancellationToken">
    /// 用于观察取消请求的 <see cref="CancellationToken"/>，若在等待期间触发，则方法将返回 (<c>false</c>, <c>default</c>)。
    /// </param>
    /// <returns>
    /// 返回一个元组：
    /// <list type="bullet">
    ///   <item><c>success</c>：若成功取出消息则为 <see langword="true"/>；否则为 <see langword="false"/>。</item>
    ///   <item><c>item</c>：取出的消息（若 <c>success</c> 为 <see langword="false"/> 则为默认值）。</item>
    /// </list>
    /// </returns>
    Task<(bool success, T? item)> TryDequeueAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 异步批量获取最多 <paramref name="batchSize"/> 条消息，针对每条消息均等待可用或取消。
    /// </summary>
    /// <param name="batchSize">
    /// 指定本次调用最多要获取的消息数量。
    /// </param>
    /// <param name="cancellationToken">
    /// 用于观察取消请求的 <see cref="CancellationToken"/>，若在等待期间触发，则停止后续等待并返回已获取的消息列表。
    /// </param>
    /// <returns>
    /// 返回一个 <see cref="List{T}"/>，包含实际取出的消息，数量最多为 <paramref name="batchSize"/>。
    /// </returns>
    Task<IEnumerable<T>> TryDequeueBatchAsync(int batchSize, CancellationToken cancellationToken);

    /// <summary>
    /// 同步批量获取最多 <paramref name="batchSize"/> 条消息。对于每条消息，操作将阻塞直到可用或取消。
    /// </summary>
    /// <param name="batchSize">
    /// 本次调用尝试获取的最大消息数量。
    /// </param>
    /// <param name="cancellationToken">
    /// 用于监视取消请求的 <see cref="CancellationToken"/>。如果在等待期间被触发，将停止进一步等待，并返回当前已获取的消息。
    /// </param>
    /// <param name="values">
    /// 输出的消息集合。
    /// </param>
    /// <returns>
    /// 如果成功获取到至少一条消息，返回 <c>true</c>，并通过 <paramref name="values"/> 输出已获取的消息集合（最多 <paramref name="batchSize"/> 条）；
    /// 否则返回 <c>false</c>。
    /// </returns>
    bool TryDequeueBatch(out IEnumerable<T> values, int batchSize, CancellationToken cancellationToken);

    /// <summary>
    /// 从队列中获取当前所有可用的消息，不等待新入队的项。
    /// </summary>
    /// <returns>
    /// 返回一个 <see cref="IList{T}"/>，包含调用时队列中所有可用的消息，且同时减少对应的信号量计数。
    /// </returns>
    IList<T> DequeueAll();

    /// <returns>
    /// 用于观察消息的 <see cref="IObservable{T}"/> 实例。
    /// </returns>
    IObservable<T>? Observable { get; }
}