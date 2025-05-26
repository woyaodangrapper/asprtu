namespace Asprtu.Memory.Interprocess.Semaphore;

internal interface IInterprocessSemaphoreReleaser : IDisposable
{
    void Release();
}