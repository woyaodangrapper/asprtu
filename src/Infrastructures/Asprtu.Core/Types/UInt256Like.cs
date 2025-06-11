using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Asprtu.Core.Types;

[StructLayout(LayoutKind.Explicit, Size = 32)]
public readonly struct UInt256Like : IEquatable<UInt256Like>
// Mark the struct as readonly to address IDE0251
{
    [FieldOffset(0)] public readonly ulong A; // Mark fields as readonly
    [FieldOffset(8)] public readonly ulong B; // Mark fields as readonly
    [FieldOffset(16)] public readonly ulong C; // Mark fields as readonly
    [FieldOffset(24)] public readonly ulong D; // Mark fields as readonly

    /// <summary>
    /// Writes up to 32 ANSI (ASCII) bytes of <paramref name="str"/> into this struct.
    /// </summary>
    public void SetFromString(string str)
    {
        Span<byte> span = MemoryMarshal.AsBytes(
            MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in this), 1) // Use "in" keyword to fix CS9195
        );
        int written = Encoding.ASCII.GetBytes(str.AsSpan(), span);
        span[written..].Clear();
    }

    public override string ToString()
    {
        ReadOnlySpan<byte> span = MemoryMarshal.AsBytes(
            MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in this), 1) // Use "in" keyword to fix CS9195
        );
        int length = span.IndexOf((byte)0);
        if (length < 0)
        {
            length = span.Length;
        }

        return Encoding.ASCII.GetString(span[..length]);
    }

    public override bool Equals([NotNullWhen(true)] object? obj) // Add nullable annotations to fix CS8765
        => obj is UInt256Like other && A == other.A && B == other.B && C == other.C && D == other.D;

    public override int GetHashCode() => HashCode.Combine(A, B, C, D);

    public static bool operator ==(UInt256Like left, UInt256Like right) => left.Equals(right);

    public static bool operator !=(UInt256Like left, UInt256Like right) => !(left == right);

    public bool Equals(UInt256Like other) => A == other.A && B == other.B && C == other.C && D == other.D;
}