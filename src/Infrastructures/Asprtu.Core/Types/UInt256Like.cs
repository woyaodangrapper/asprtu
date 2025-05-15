using System.Runtime.InteropServices;
using System.Text;

namespace Asprtu.Core.Types;

[StructLayout(LayoutKind.Explicit, Size = 32)]
public struct UInt256Like
{
    [FieldOffset(0)] public ulong A;
    [FieldOffset(8)] public ulong B;
    [FieldOffset(16)] public ulong C;
    [FieldOffset(24)] public ulong D;

    /// <summary>
    /// Writes up to 32 ANSI (ASCII) bytes of <paramref name="str"/> into this struct.
    /// </summary>
    public void SetFromString(string str)
    {
        // Get a Span<byte> view over the 32 bytes of this struct
        Span<byte> span = MemoryMarshal.AsBytes(
            MemoryMarshal.CreateSpan(ref this, 1)
        );
        // Encode directly into the struct’s bytes
        int written = Encoding.ASCII.GetBytes(str.AsSpan(), span);
        // Zero out any remaining bytes
        span[written..].Clear();
    }

    public override string ToString()
    {
        // Get the same Span<byte> view
        ReadOnlySpan<byte> span = MemoryMarshal.AsBytes(
            MemoryMarshal.CreateSpan(ref this, 1)
        );
        // Find the zero terminator (or take all 32)
        int length = span.IndexOf((byte)0);
        if (length < 0) length = span.Length;
        // Decode only the meaningful prefix
        return Encoding.ASCII.GetString(span[..length]);
    }
}