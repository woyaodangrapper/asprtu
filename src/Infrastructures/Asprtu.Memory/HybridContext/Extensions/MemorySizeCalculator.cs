using System.Runtime.InteropServices;

namespace Asprtu.Memory.HybridContext.Extensions;

public static class MemorySizeCalculator
{
    internal static int TryOccupy<T>(T item)
    {
        try
        {
            int size = Marshal.SizeOf(item);

            if (size <= 64)
                return 1;

            int occupy = (int)Math.Ceiling(size / (double)1);
            return occupy;
        }
        catch (ArgumentException)
        {
            // Handle specific exception related to Marshal.SizeOf
            return 1;
        }
        catch (InvalidOperationException)
        {
            // Handle specific exception if any invalid operation occurs
            return 1;
        }
    }
}