using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Asprtu.Gens.Inspectors;

public class AttributedCrossClassTransformer(string attributeFullName) : IPostCollectSyntaxTransformer
{
    private readonly string _attributeFullName = attributeFullName;

    public ImmutableArray<SyntaxInfo> Transform(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos)
    {
        var result = ImmutableArray.CreateBuilder<SyntaxInfo>();

        var attributeSymbol = compilation.GetTypeByMetadataName(_attributeFullName);
        if (attributeSymbol == null)
            return result.ToImmutable();

        var assembliesToSearch = new List<IAssemblySymbol> { compilation.Assembly };
        assembliesToSearch.AddRange(compilation.SourceModule.ReferencedAssemblySymbols);

        var addedKeys = new HashSet<string>();
        foreach (var assembly in assembliesToSearch
                    .Distinct(SymbolEqualityComparer.Default)
                    .Cast<IAssemblySymbol>())
        {
            SearchNamespace(assembly.GlobalNamespace, attributeSymbol, result, ref addedKeys);
        }

        return result.ToImmutable();
    }

    private void SearchNamespace(
        INamespaceSymbol namespaceSymbol,
        INamedTypeSymbol attributeSymbol,
        ImmutableArray<SyntaxInfo>.Builder result,
        ref HashSet<string> addedKeys)
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
            SearchNamespace(subNs, attributeSymbol, result, ref addedKeys);
        }
    }
}