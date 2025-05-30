using Microsoft.CodeAnalysis;
using resource_analyzers.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace resource_analyzers.Inspectors;

public class AttributedCrossClassTransformer(string attributeFullName) : IPostCollectSyntaxTransformer
{
    private readonly string _attributeFullName = attributeFullName;

    public ImmutableArray<SyntaxInfo> Transform(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos)
    {
        var result = syntaxInfos.ToBuilder();
        var addedKeys = new HashSet<string>(syntaxInfos.Select(si => si.OrderByKey));
        var attributeSymbol = compilation.GetTypeByMetadataName(_attributeFullName);
        if (attributeSymbol == null)
            return result.ToImmutable();

        var assembliesToSearch = new List<IAssemblySymbol> { compilation.Assembly };
        assembliesToSearch.AddRange(compilation.SourceModule.ReferencedAssemblySymbols);

        foreach (IAssemblySymbol assembly in assembliesToSearch.Distinct(SymbolEqualityComparer.Default).Cast<IAssemblySymbol>())
        {
            SearchNamespace(assembly.GlobalNamespace, attributeSymbol, result, addedKeys);
        }

        return result.ToImmutable();
    }

    private void SearchNamespace(
        INamespaceSymbol namespaceSymbol,
        INamedTypeSymbol attributeSymbol,
        ImmutableArray<SyntaxInfo>.Builder result,
        HashSet<string> addedKeys)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            if (type.TypeKind != TypeKind.Class)
                continue;

            if (type.GetAttributes().Any(attr =>
                SymbolEqualityComparer.Default.Equals(attr.AttributeClass, attributeSymbol)))
            {
                var implFullName = type.ToDisplayString();
                var interfaceFullName = $"{type.ContainingNamespace}.{type.Name}";
                var typeInfo = new LibraryInfo(implFullName, interfaceFullName);
                if (addedKeys.Add(typeInfo.OrderByKey))
                {
                    result.Add(typeInfo);
                }
            }
        }

        foreach (var subNs in namespaceSymbol.GetNamespaceMembers())
        {
            SearchNamespace(subNs, attributeSymbol, result, addedKeys);
        }
    }
}