using Asprtu.Capacities.EventHub.Mqtt.Configuration;

namespace Asprtu.Capacities.EventHub.Mqtt.Contracts;

/// <summary>
/// 提供统一的 MQTT 发布接口，支持基础数据类型和泛型消息的发布，适用于 AOT 兼容的高性能消息传输场景。
/// </summary>
public interface IMqttPub
{
    /// <summary>
    /// 以字符串形式发布原始 MQTT 消息。
    /// </summary>
    /// <param name="topic">目标主题（Topic）。</param>
    /// <param name="payload">消息负载（字符串格式）。</param>
    /// <param name="retain">是否保留消息（Retain Flag）。</param>
    /// <param name="qos">服务质量等级（0、1 或 2）。</param>
    Task PublishAsync(string topic, string payload, bool retain = false, int qos = 1);

    /// <summary>
    /// 发布整数类型的数据。
    /// </summary>
    Task<bool> TryPublishAsync(int data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布原始字节数组数据。
    /// </summary>
    Task<bool> TryPublishAsync(byte[] data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布单精度浮点数据。
    /// </summary>
    Task<bool> TryPublishAsync(float data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布双精度浮点数据。
    /// </summary>
    Task<bool> TryPublishAsync(double data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布布尔类型的数据。
    /// </summary>
    Task<bool> TryPublishAsync(bool data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布短整型数据。
    /// </summary>
    Task<bool> TryPublishAsync(short data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布长整型数据。
    /// </summary>
    Task<bool> TryPublishAsync(long data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布单字节数据。
    /// </summary>
    Task<bool> TryPublishAsync(byte data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布字符类型的数据。
    /// </summary>
    Task<bool> TryPublishAsync(char data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布高精度十进制数据。
    /// </summary>
    Task<bool> TryPublishAsync(decimal data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布字符串类型数据。
    /// </summary>
    Task<bool> TryPublishAsync(string data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布日期时间数据。
    /// </summary>
    Task<bool> TryPublishAsync(DateTime data, MqttPubRule? rule = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布泛型消息类型，要求其继承自 <see cref="MqttAbstractMessage"/>，可用于支持多态序列化和 AOT 优化。
    /// </summary>
    /// <typeparam name="T">具体的 MQTT 消息类型，必须继承自 <see cref="MqttAbstractMessage"/> 并具有无参构造函数。</typeparam>
    /// <param name="data">消息对象实例。</param>
    /// <param name="rule">可选的发布规则（Topic、QoS、保留标志等）。</param>
    /// <param name="cancellationToken">用于取消操作的标记。</param>
    Task<bool> TryPublishAsync<T>(T data, MqttPubRule? rule = null, CancellationToken cancellationToken = default)
        where T : MqttAbstractMessage, new();
}