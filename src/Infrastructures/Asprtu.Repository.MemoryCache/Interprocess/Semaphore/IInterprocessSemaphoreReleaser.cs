namespace Asprtu.Repository.MemoryCache;

internal interface IInterprocessSemaphoreReleaser : IDisposable
{
    void Release();
}