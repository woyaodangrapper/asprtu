using System;

namespace Asprtu.Gens.Models;

public sealed class FactoryInfo : SyntaxInfo
{
    public override string OrderByKey => $"{InterfaceName}<{CapabilityType}>";

    /// <summary>
    /// ʵ������������ƣ����� "MyApp.Factories.TcpClientFactory"
    /// </summary>
    public string TypeName { get; }

    /// <summary>
    /// ʵ�ֵĽӿ�����"ILibraryFactory" �� "ILibraryCapacitiesFactory"
    /// </summary>
    public string InterfaceName { get; }

    /// <summary>
    /// ���Ͳ������͵��������ƣ����� "MyApp.Contracts.TcpClient"
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