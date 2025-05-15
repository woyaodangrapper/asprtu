using System.Buffers;
using System.Runtime.InteropServices;

namespace Asprtu.Core.Protocol;

[StructLayout(LayoutKind.Explicit, Size = 20)]
public struct HeartBeating : IEquatable<HeartBeating>
{
    private static readonly ArrayPool<byte> _bytePool = ArrayPool<byte>.Shared;

    [FieldOffset(0)]
    public ushort Version;

    [FieldOffset(2)]
    public ulong Timestamp;

    [FieldOffset(10)]
    public ulong UserSecretsId;

    [FieldOffset(18)]
    public ushort HeartbeatStatus;

    public HeartBeating(byte[] bytes)
    {
        this = default;

        var span = bytes.AsSpan();
        Version = BitConverter.ToUInt16(span[..2]);             // 偏移量 0，占2字节
        Timestamp = BitConverter.ToUInt64(span.Slice(2, 8));           // 偏移量 2，占8字节
        UserSecretsId = BitConverter.ToUInt64(span.Slice(10, 8));      // 偏移量 10，占8字节
        HeartbeatStatus = BitConverter.ToUInt16(span.Slice(18, 2));    // 偏移量 18，占2字节
    }

    public HeartBeating(ReadOnlySpan<byte> bytes)
    {
        this = default;
        Version = BitConverter.ToUInt16(bytes[..2]);             // 偏移量 0，占2字节
        Timestamp = BitConverter.ToUInt64(bytes.Slice(2, 8));           // 偏移量 2，占8字节
        UserSecretsId = BitConverter.ToUInt64(bytes.Slice(10, 8));      // 偏移量 10，占8字节
        HeartbeatStatus = BitConverter.ToUInt16(bytes.Slice(18, 2));    // 偏移量 18，占2字节
    }

    public HeartBeating(ReadOnlyMemory<byte> bytes)
    {
        this = default;
        ReadOnlySpan<byte> span = bytes.Span;
        Version = BitConverter.ToUInt16(span[..2]);             // 偏移量 0，占2字节
        Timestamp = BitConverter.ToUInt64(span.Slice(2, 8));           // 偏移量 2，占8字节
        UserSecretsId = BitConverter.ToUInt64(span.Slice(10, 8));      // 偏移量 10，占8字节
        HeartbeatStatus = BitConverter.ToUInt16(span.Slice(18, 2));    // 偏移量 18，占2字节
    }

    public HeartBeating(int? version = null, long? timestamp = null, long? userSecretsId = null, int? heartbeatStatus = null)
    {
        this = default;

        Version = (ushort)(version ?? 0);
        Timestamp = (ulong)(timestamp ?? DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        UserSecretsId = (ulong)(userSecretsId ?? 0);
        HeartbeatStatus = (ushort)(heartbeatStatus ?? 1);
    }

    /// <summary>
    /// 序列化为字节数组
    /// </summary>
    public readonly HeartBeating ToBytes(out byte[] buffer)
    {
        // 从池中租用一个字节数组,请求一个大小为 20 的数组 (一个标准块大小，可能是 16、32、64……等)
        buffer = _bytePool.Rent(20);

        try
        {
            var span = buffer.AsSpan();
            BitConverter.TryWriteBytes(span[..2], Version);
            BitConverter.TryWriteBytes(span.Slice(2, 8), Timestamp);
            BitConverter.TryWriteBytes(span.Slice(10, 8), UserSecretsId);
            BitConverter.TryWriteBytes(span.Slice(18, 2), HeartbeatStatus);
        }
        catch
        {
            // 确保在发生异常时返回字节数组给池中
            _bytePool.Return(buffer);
            throw;
        }
        return this;
    }

    public override readonly bool Equals(object? obj) // 修改为 object? 以匹配为 Null 性
    {
        return obj is HeartBeating other && Equals(other);
    }

    public readonly bool Equals(HeartBeating other)
    {
        return Version == other.Version &&
               Timestamp == other.Timestamp &&
               UserSecretsId == other.UserSecretsId &&
               HeartbeatStatus == other.HeartbeatStatus;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Version, Timestamp, UserSecretsId, HeartbeatStatus);
    }

    public static bool operator ==(HeartBeating left, HeartBeating right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(HeartBeating left, HeartBeating right)
    {
        return !(left == right);
    }
}