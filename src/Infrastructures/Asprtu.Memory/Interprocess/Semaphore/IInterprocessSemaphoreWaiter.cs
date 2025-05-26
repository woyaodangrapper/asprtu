namespace Asprtu.Memory.Interprocess.Semaphore;

internal interface IInterprocessSemaphoreWaiter : IDisposable
{
    bool Wait(int millisecondsTimeout);
}