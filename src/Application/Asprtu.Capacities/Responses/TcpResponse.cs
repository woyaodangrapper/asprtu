namespace Asprtu.Capacities.Responses;

/// <summary>
/// TCP 连接的响应信息，用于 API 返回数据结构
/// Response data for a TCP connection, used in API output
/// </summary>
public class TcpResponse : AppAbstractMessage
{
    /// <summary>
    /// 连接唯一标识符
    /// Unique connection identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 客户端应用名称
    /// Name of the client application
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 本地监听端口号（字符串格式）
    /// Local listening port (as string)
    /// </summary>
    public string Port { get; set; } = string.Empty;

    /// <summary>
    /// 使用的协议或版本号
    /// Protocol used or version string
    /// </summary>
    public string Protocol { get; set; } = string.Empty;

    /// <summary>
    /// 当前连接是否处于运行状态
    /// Whether the connection is currently active
    /// </summary>
    public bool IsRunning { get; set; }

    /// <summary>
    /// 连接建立时间（带时区）
    /// Timestamp when the connection was established (with offset)
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 最后活跃时间（带时区）
    /// Timestamp of the last activity (with offset)
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }
}