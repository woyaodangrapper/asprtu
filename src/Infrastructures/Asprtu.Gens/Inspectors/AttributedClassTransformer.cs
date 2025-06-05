using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Asprtu.Gens.Inspectors;

public class AttributedCrossClassTransformer(IEnumerable<string> attributeFullNames) : IPostCollectSyntaxTransformer
{
    private readonly IEnumerable<string> _attributeFullNames = new HashSet<string>(attributeFullNames);

    public ImmutableArray<SyntaxInfo> Transform(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos)
    {
        var result = ImmutableArray.CreateBuilder<SyntaxInfo>();

        var attributeSymbols = _attributeFullNames
           .Select(name => compilation.GetTypeByMetadataName(name))
           .OfType<INamedTypeSymbol>()// 自动去除 null 并转换类型
           .ToImmutableHashSet(SymbolEqualityComparer.Default);

        if (attributeSymbols.Count == 0)
            return result.ToImmutable();

        var assembliesToSearch = new List<IAssemblySymbol> { compilation.Assembly };
        assembliesToSearch.AddRange(compilation.SourceModule.ReferencedAssemblySymbols);

        var addedKeys = new HashSet<string>();
        foreach (var assembly in assembliesToSearch
                     .Distinct(SymbolEqualityComparer.Default)
                     .Where(a => a?.Name.StartsWith("Asprtu.Rtu.", StringComparison.Ordinal) ?? false)
                     .Cast<IAssemblySymbol>())
        {
            SearchNamespace(assembly.GlobalNamespace, attributeSymbols, result, ref addedKeys);
        }
        return result.ToImmutable();
    }

    private void SearchNamespace(
      INamespaceSymbol namespaceSymbol,
      ImmutableHashSet<ISymbol?>? attributeSymbols,
      ImmutableArray<SyntaxInfo>.Builder result,
      ref HashSet<string> addedKeys)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            if (type.TypeKind != TypeKind.Class || type.IsAbstract)
                continue;

            if (type.GetAttributes().Any(attr =>
                attr.AttributeClass != null &&
                attributeSymbols.Contains(attr.AttributeClass, SymbolEqualityComparer.Default)))
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

        // 递归进入子命名空间
        foreach (var subNs in namespaceSymbol.GetNamespaceMembers())
        {
            SearchNamespace(subNs, attributeSymbols, result, ref addedKeys);
        }
    }
}