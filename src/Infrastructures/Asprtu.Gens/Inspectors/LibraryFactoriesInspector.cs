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
        public ImmutableArray<ISyntaxFilter> Filters { get; } = [ClassWithAnyInterface.Instance];

        public IImmutableSet<SyntaxKind> SupportedKinds { get; } =
            [SyntaxKind.ClassDeclaration];

        public bool TryHandle(
            GeneratorSyntaxContext context,
            [NotNullWhen(true)] out SyntaxInfo? syntaxInfo)
        {
            syntaxInfo = null;

            if (context.Node is not ClassDeclarationSyntax classDecl)
                return false;

            if (context.SemanticModel.GetDeclaredSymbol(classDecl) is not INamedTypeSymbol typeSymbol)
                return false;

            if (typeSymbol.AllInterfaces.Length == 0)
                return false;

            foreach (var attribute in classDecl.AttributeLists.SelectMany(a => a.Attributes))
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is IMethodSymbol methodSymbol)
                {
                    var implFullName = typeSymbol.ToDisplayString();
                    var interfaceFullName = $"{typeSymbol.ContainingNamespace}.{typeSymbol.Name}";
                    syntaxInfo = new LibraryInfo(implFullName, interfaceFullName);
                    return true;
                }
            }
            syntaxInfo = default;
            return false;
        }
    }
}