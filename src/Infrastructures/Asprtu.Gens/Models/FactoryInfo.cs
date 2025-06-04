using System;

namespace Asprtu.Gens.Models;

public sealed class FactoryInfo : SyntaxInfo
{
    public override string OrderByKey => $"{InterfaceName}<{CapabilityType}>";

    /// <summary>
    /// 实现类的完整名称，例如 "MyApp.Factories.TcpClientFactory"
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// 实现的接口名，"ILibraryFactory" 或 "ILibraryCapacitiesFactory"
    /// </summary>
    public string InterfaceName { get; }

    /// <summary>
    /// 泛型参数类型的完整名称，例如 "MyApp.Contracts.TcpClient"
    /// </summary>
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