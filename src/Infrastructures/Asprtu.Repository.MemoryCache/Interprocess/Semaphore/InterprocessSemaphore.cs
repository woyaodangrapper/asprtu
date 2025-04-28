using Asprtu.Repository.MemoryCache.Semaphore.Linux;
using Asprtu.Repository.MemoryCache.Semaphore.MacOS;
using Asprtu.Repository.MemoryCache.Semaphore.Windows;
using System.Runtime.InteropServices;

namespace Asprtu.Repository.MemoryCache;

/// <summary>
/// This class opens or creates platform agnostic named semaphore. Named
/// semaphores are synchronization constructs accessible across processes.
/// </summary>
internal static class InterprocessSemaphore
{
    internal static IInterprocessSemaphoreWaiter CreateWaiter(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new SemaphoreWindows(name);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return new SemaphoreMacOS(name);

        return new SemaphoreLinux(name);
    }

    internal static IInterprocessSemaphoreReleaser CreateReleaser(string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return new SemaphoreWindows(name);

        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return new SemaphoreMacOS(name);

        return new SemaphoreLinux(name);
    }
}