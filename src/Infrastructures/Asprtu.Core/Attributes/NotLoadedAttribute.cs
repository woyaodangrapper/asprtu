namespace Asprtu.Core.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public sealed class NotLoadedAttribute : Attribute
{
}