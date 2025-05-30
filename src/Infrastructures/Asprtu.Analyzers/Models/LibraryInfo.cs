namespace resource_analyzers.Models;

public sealed class LibraryInfo : SyntaxInfo
{
    public override string OrderByKey => TypeName;

    public string TypeName { get; }

    public string InterfaceName { get; }

    public LibraryInfo(string typeName, string interfaceName)
    {
        TypeName = typeName;
        InterfaceName = interfaceName;
    }

    public override bool Equals(SyntaxInfo? other)
        => other is LibraryInfo o
           && TypeName == o.TypeName
           && InterfaceName == o.InterfaceName;

    public override int GetHashCode() => OrderByKey.GetHashCode();
}