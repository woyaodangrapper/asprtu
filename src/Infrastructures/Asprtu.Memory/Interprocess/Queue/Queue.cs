using Asprtu.Memory.Interprocess.Contracts;
using Asprtu.Memory.Interprocess.Memory;
using System.Runtime.CompilerServices;

namespace Asprtu.Memory.Interprocess.Queue;

internal abstract class Queue : IDisposable
{
    private readonly MemoryView view;

    protected unsafe Queue(QueueOptions options, ILoggerFactory loggerFactory)
    {
        Logger = loggerFactory.CreateLogger<Queue>();
        view = new MemoryView(options, loggerFactory);
        try
        {
            Buffer = new CircularBuffer(sizeof(QueueHeader) + view.Pointer, options.Capacity);
        }
        catch
        {
            view.Dispose();
            throw;
        }

        // must clean up if the application is being closed but finalizer is not called.
        // this happens in cases such as closing a console app by pressing the X button.
        AppDomain.CurrentDomain.ProcessExit += OnAppExit;
        Console.CancelKeyPress += OnAppExit;
    }

    ~Queue() =>
        Dispose(false);

    public unsafe QueueHeader* Header
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (QueueHeader*)view.Pointer;
    }

    protected CircularBuffer Buffer { get; }
    protected ILogger<Queue> Logger { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            view.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static unsafe long GetMessageBodyOffset(long startOffset) =>
        startOffset + sizeof(MessageHeader);

    /// <summary>
    /// Calculates the total length of a message which consists of [header][body][padding].
    /// <list type="bullet">
    /// <item><term>header</term><description>An instance of <see cref="MessageHeader"/></description></item>
    /// <item><term>body</term><description>A collection of bytes provided by the user</description></item>
    /// <item><term>padding</term><description>A possible padding is added to round up the length to the closest multiple of 8 bytes</description></item>
    /// </list>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static unsafe long GetPaddedMessageLength(long bodyLength)
    {
        var length = sizeof(MessageHeader) + bodyLength;

        // Round up to the closest integer divisible by 8. This will add the [padding] if one is needed.
        return 8 * (long)Math.Ceiling(length / 8.0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected long SafeIncrementMessageOffset(long offset, long increment) =>
        (offset + increment) % (Buffer.Capacity * 2);

    private void OnAppExit(object? sender, EventArgs e)
    {
        try
        {
            Dispose(false);
        }
        catch
        {
        }
    }
}