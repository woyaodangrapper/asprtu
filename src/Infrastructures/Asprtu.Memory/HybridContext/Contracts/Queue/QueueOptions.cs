namespace Asprtu.Memory.HybridContext.Contracts.Queue;

/// <summary>
/// 队列配置选项
/// </summary>
public sealed class QueueOptions(string name)
{
    public QueueOptions(string name, bool mode)
        : this(name)
    {
        Name = name;
        Mode = mode;
    }

    /// <summary> 队列名称 </summary>
    public string Name { get; set; } = name;

    /// <summary> 缓存最大条数 </summary>
    public long SizeLimit { get; set; } = 1000;

    /// <summary> 过期缓存扫描频率 </summary>
    public TimeSpan ExpirationScanFrequency { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary> 达到大小限制时的压缩比例 </summary>
    public double CompactionPercentage { get; set; } = 0.2;

    /// <summary> 极限模式：是否直接使用 T 类型缓存 </summary>
    public bool Mode { get; set; }

    /// <summary> 单条缓存默认存活时间 </summary>
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary> 是否启用 Fail-Safe 容错机制 </summary>
    public bool IsFailSafeEnabled { get; set; } = true;

    /// <summary> 容错限流间隔时间 </summary>
    public TimeSpan FailSafeThrottleDuration { get; set; } = TimeSpan.FromSeconds(5);
}