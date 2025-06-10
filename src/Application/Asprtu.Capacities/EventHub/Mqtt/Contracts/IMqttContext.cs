using Asprtu.Memory.HybridContext.Contracts;

namespace Asprtu.Capacities.EventHub.Mqtt.Contracts;

/// <summary>
/// 表示一个用于管理和分发 MQTT 消息的上下文接口，定义实体变更的跟踪与分发行为。
/// </summary>
/// <typeparam name="T">
/// 与该上下文关联的实体类型，必须继承自 <see cref="MqttAbstractMessage"/> 且具有无参构造函数。
/// </typeparam>
public interface IMqttContext<T> : IDisposable
    where T : MqttAbstractMessage, new()
{
    /// <summary>
    /// 跟踪新实体为 Added。
    /// </summary>
    void Add(T entity);

    /// <summary>
    /// 跟踪实体为 Updated。
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// 跟踪实体为 Removed。
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// 实现 DispatchAsync，将消息序列化并通过 MQTT 发布
    /// </summary>
    Task DispatchAsync(T entity, ChangeType changeType);

    /// <summary>
    /// 批量提交所有已跟踪的变更。
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}