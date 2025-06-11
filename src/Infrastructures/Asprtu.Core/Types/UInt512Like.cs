using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;

namespace Asprtu.Core.Types;

[StructLayout(LayoutKind.Explicit, Size = 64)]
public struct UInt512Like : IEquatable<UInt512Like>
{
    [FieldOffset(0)] public UInt256Like Part1;
    [FieldOffset(32)] public UInt256Like Part2;

    public void SetFromString(string str)
    {
        Span<byte> span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1));
        int written = Encoding.ASCII.GetBytes(str.AsSpan(), span);
        span[written..].Clear();
    }

    public override string ToString()
    {
        ReadOnlySpan<byte> span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref this, 1));
        int length = span.IndexOf((byte)0);
        if (length < 0)
        {
            length = span.Length;
        }

        return Encoding.ASCII.GetString(span[..length]);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj) => obj is UInt512Like other && Equals(other);

    public override readonly int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Part1);
        hash.Add(Part2);
        return hash.ToHashCode();
    }

    public static bool operator ==(UInt512Like left, UInt512Like right) => left.Equals(right);

    public static bool operator !=(UInt512Like left, UInt512Like right) => !(left == right);

    public readonly bool Equals(UInt512Like other) =>
        Part1.Equals(other.Part1) && Part2.Equals(other.Part2);
}