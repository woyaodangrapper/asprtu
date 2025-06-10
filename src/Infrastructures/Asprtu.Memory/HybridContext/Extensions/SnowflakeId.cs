namespace System;

public class SnowflakeId
{
    // 雪花算法的起始时间（自定义起始时间）
    private static readonly long _epoch = new DateTime(2023, 1, 1).ToUniversalTime().Ticks / 10000;

    // 机器ID占用的位数
    private const int MachineIdBits = 10;

    // 序列号占用的位数
    private const int SequenceBits = 12;

    // 机器ID最大值 (1023)
    private const long MaxMachineId = -1L ^ (-1L << MachineIdBits);

    // 序列号最大值 (4095)
    private const long MaxSequence = -1L ^ (-1L << SequenceBits);

    // 机器ID偏移量 (12位序列号 + 41位时间戳)
    private const int MachineIdShift = SequenceBits;

    // 时间戳偏移量 (12位序列号 + 10位机器ID)
    private const int TimestampLeftShift = SequenceBits + MachineIdBits;

    // 雪花ID的序列号
    private long _sequence;

    // 上次生成ID的时间戳
    private long _lastTimestamp = -1L;

    // 当前机器ID（基于当前程序集的GUID）
    private long _machineId;

    // 锁对象，确保线程安全
    private static readonly object _lock = new();

    // 静态生成器对象
    private static SnowflakeId? _instance;

    // 构造函数，自动根据应用程序 GUID 生成机器 ID
    private SnowflakeId(bool isMicro)
    {
        _machineId = (isMicro ? Environment.ProcessId.GetHashCode() : GetUniqueMachineId()) & MaxMachineId;

        if (_machineId < 0 || _machineId > MaxMachineId)
        {
            throw new ArgumentException($"机器ID超出范围。生成的机器ID必须在 0 到 {MaxMachineId} 之间");
        }
    }

    public static SnowflakeId Instance => _instance ??= new SnowflakeId(true);

    // 静态方法用于生成雪花ID，简化使用
    public static long NewSnowflakeId() => Instance.GenerateId();

    // 生成一个 Snowflake ID
    private long GenerateId()
    {
        lock (_lock)
        {
            // 获取当前时间戳
            var timestamp = GetCurrentTimestamp();

            // 如果当前时间戳与上次生成ID的时间相同，生成序列号
            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                {
                    // 如果序列号溢出，等待下一毫秒
                    timestamp = WaitForNextMillis(_lastTimestamp);
                }
            }
            else
            {
                // 重置序列号
                _sequence = 0L;
            }

            // 更新最后生成ID的时间戳
            _lastTimestamp = timestamp;

            // 计算最终的Snowflake ID
            ulong id = ((ulong)(timestamp - _epoch) << TimestampLeftShift) |
                  ((ulong)_machineId << MachineIdShift) |
                  (ulong)_sequence;

            return (long)(id & 0x7FFFFFFFFFFFFFFF);
        }
    }

    // 获取当前时间戳（毫秒级）
    private static long GetCurrentTimestamp()
    {
        return (DateTime.UtcNow.Ticks / 10000) - _epoch;
    }

    // 如果当前时间戳与上次生成ID的时间相同，则等待下一毫秒
    private static long WaitForNextMillis(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }

    private static int GetUniqueMachineId()
    {
        return $"{Environment.MachineName}-{Environment.UserDomainName}-{Environment.OSVersion.Version}"
               .GetHashCode(StringComparison.Ordinal);
    }
}