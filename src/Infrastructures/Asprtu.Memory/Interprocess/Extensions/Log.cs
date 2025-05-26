using Asprtu.Memory.Interprocess.Memory;

namespace Asprtu.Repository.MemoryCache;

internal static partial class Log
{
    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to delete queue's shared memory backing file even though it is not in use by any process.")]
    internal static partial void FailedToDeleteSharedMemoryFile(
        this ILogger<MemoryFileUnix> logger);
}