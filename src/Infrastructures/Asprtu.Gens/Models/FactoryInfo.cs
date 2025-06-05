using System;

namespace Asprtu.Gens.Models;

public sealed class FactoryInfo : SyntaxInfo
{
    public override string OrderByKey => $"{InterfaceName}<{CapabilityType}>";

    public string TypeName { get; }

    public string InterfaceName { get; }

    public string CapabilityType { get; }

    public FactoryInfo(string typeName, string interfaceName, string capabilityType)
    {
        TypeName = typeName;
        InterfaceName = interfaceName;
        CapabilityType = capabilityType;
    }

    public override bool Equals(SyntaxInfo? other) =>
        other is FactoryInfo o &&
        TypeName == o.TypeName &&
        InterfaceName == o.InterfaceName &&
        CapabilityType == o.CapabilityType;

    public override int GetHashCode() =>
        HashCode.Combine(TypeName, InterfaceName, CapabilityType);
}