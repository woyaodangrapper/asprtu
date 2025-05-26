namespace Asprtu.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AsprtusAttribute : Attribute
{
    public string Description { get; } = string.Empty;

    public AsprtusAttribute()
    {
    }

    public AsprtusAttribute(string _description) => Description = _description;
}