namespace Asprtu.Memory.HybridContext.Contracts;

/// <summary>
/// Encapsulates a tracked change entry.
/// </summary>
public class ChangeEntry<T>(T entity, ChangeType type)
{
    public T Entity { get; } = entity;
    public ChangeType Type { get; } = type;
}