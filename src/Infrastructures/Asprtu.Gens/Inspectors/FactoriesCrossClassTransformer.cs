using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Asprtu.Gens.Inspectors;

public class FactoriesCrossClassTransformer : IPostCollectSyntaxTransformer
{
    private static readonly string[] TargetInterfaces =
    [
        "ILibraryFactory`1",
        "ILibraryCapacitiesFactory`1"
    ];

    public ImmutableArray<SyntaxInfo> Transform(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos)
    {
        var result = ImmutableArray.CreateBuilder<SyntaxInfo>();

        var assembliesToSearch = new List<IAssemblySymbol> { compilation.Assembly };
        assembliesToSearch.AddRange(compilation.SourceModule.ReferencedAssemblySymbols);

        var addedKeys = new HashSet<string>();

        foreach (var assembly in assembliesToSearch
                     .Distinct(SymbolEqualityComparer.Default)
                     .Where(a => a?.Name.StartsWith("Asprtu.Rtu.", StringComparison.Ordinal) ?? false)
                     .Cast<IAssemblySymbol>())
        {
            SearchNamespace(assembly.GlobalNamespace, result, ref addedKeys, compilation);
        }
        return result.ToImmutable();
    }

    private void SearchNamespace(
        INamespaceSymbol namespaceSymbol,
        ImmutableArray<SyntaxInfo>.Builder result,
        ref HashSet<string> addedKeys,
        Compilation compilation)
    {
        foreach (var type in namespaceSymbol.GetTypeMembers())
        {
            if (type.TypeKind != TypeKind.Class || type.IsAbstract)
                continue;

            if (ImplementsTargetInterface(type, out string? capabilityType))
            {
                var implFullName = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var interfaceFullName = type.ConstructedFrom.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                var typeInfo = new FactoryInfo(implFullName, interfaceFullName, capabilityType ?? string.Empty);
                if (addedKeys.Add(typeInfo.OrderByKey))
                {
                    result.Add(typeInfo);
                }
            }
        }

        foreach (var subNs in namespaceSymbol.GetNamespaceMembers())
        {
            SearchNamespace(subNs, result, ref addedKeys, compilation);
        }
    }

    private static bool ImplementsTargetInterface(INamedTypeSymbol type, out string? genericArgName)
    {
        genericArgName = null;

        foreach (var iface in type.AllInterfaces)
        {
            var ifaceDef = iface.OriginalDefinition;
            var nameWithArity = ifaceDef.Name + "`" + ifaceDef.Arity;
            if (TargetInterfaces.Contains(nameWithArity))
            {
                var typeArg = iface.TypeArguments.FirstOrDefault();
                if (typeArg != null)
                {
                    genericArgName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                }
                return true;
            }
        }

        return false;
    }
}