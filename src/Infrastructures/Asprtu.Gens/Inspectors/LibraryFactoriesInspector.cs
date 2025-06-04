using Asprtu.Gens.Filters;
using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Asprtu.Gens.Inspectors
{
    public sealed class LibraryFactoriesInspector : ISyntaxInspector
    {
        public ImmutableArray<ISyntaxFilter> Filters { get; } = [];

        public IImmutableSet<SyntaxKind> SupportedKinds { get; } =
            [SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration];

        public bool TryHandle(
            GeneratorSyntaxContext context,
            [NotNullWhen(true)] out SyntaxInfo? syntaxInfo)
        {
            syntaxInfo = null;

            if (context.Node is not TypeDeclarationSyntax typeDecl)
                return false;

            if (context.SemanticModel.GetDeclaredSymbol(typeDecl) is not INamedTypeSymbol typeSymbol)
                return false;

            foreach (var iface in typeSymbol.AllInterfaces)
            {
                if (iface.OriginalDefinition.ToDisplayString() is
                    "ILibraryFactory<T>" or "ILibraryCapacitiesFactory<T>")
                {
                    // 提取泛型参数
                    var typeArg = iface.TypeArguments.FirstOrDefault();
                    var capabilityType = typeArg?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? "unknown";

                    syntaxInfo = new FactoryInfo(
                         typeName: typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                         interfaceName: iface.OriginalDefinition.Name,
                         capabilityType: iface.TypeArguments.First().ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                     );
                    return true;
                }
            }

            return false;
        }
    }
}