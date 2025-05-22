namespace Aspire.Contracts;

internal interface INotifySuperiors
{
    Task<bool> RegisterAsync(CancellationToken cancellationToken = default);

    Task<bool> TryRegisterAsync(CancellationToken cancellationToken = default);
}