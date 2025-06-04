using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Asprtu.Gens.Inspectors;

public class FactoriesCrossClassTransformer : IPostCollectSyntaxTransformer
{
    private static readonly string[] TargetInterfaces = new[]
    {
        "ILibraryFactory<T>",
        "ILibraryCapacitiesFactory<T>"
    };

    public ImmutableArray<SyntaxInfo> Transform(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos)
    {
        var result = ImmutableArray.CreateBuilder<SyntaxInfo>();

        //var assembliesToSearch = new List<IAssemblySymbol> { compilation.Assembly };
        //assembliesToSearch.AddRange(compilation.SourceModule.ReferencedAssemblySymbols);

        //var addedKeys = new HashSet<string>();

        //foreach (var assembly in assembliesToSearch
        //             .Distinct(SymbolEqualityComparer.Default))
        //{
        //    SearchNamespace(assembly.GlobalNamespace, result, ref addedKeys, compilation);
        //}

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
            if (type.TypeKind != TypeKind.Class)
                continue;
        }

        foreach (var subNs in namespaceSymbol.GetNamespaceMembers())
        {
            SearchNamespace(subNs, result, ref addedKeys, compilation);
        }
    }
}