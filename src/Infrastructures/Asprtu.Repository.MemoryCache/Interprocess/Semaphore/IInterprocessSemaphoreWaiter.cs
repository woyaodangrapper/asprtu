namespace Asprtu.Repository.MemoryCache;

internal interface IInterprocessSemaphoreWaiter : IDisposable
{
    bool Wait(int millisecondsTimeout);
}