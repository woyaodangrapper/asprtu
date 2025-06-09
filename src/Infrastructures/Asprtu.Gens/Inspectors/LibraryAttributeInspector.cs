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
    public sealed class LibraryAttributeInspector : ISyntaxInspector
    {
        public ImmutableArray<ISyntaxFilter> Filters { get; } = [TypeWithAttribute.Instance];

        // 支持处理的节点类型：类、结构、接口、枚举
        public IImmutableSet<SyntaxKind> SupportedKinds { get; } =
        [
            SyntaxKind.ClassDeclaration
        ];

        public bool TryHandle(
            GeneratorSyntaxContext context,
            [NotNullWhen(true)] out SyntaxInfo? syntaxInfo)
        {
            syntaxInfo = null;

            if (context.Node is not BaseTypeDeclarationSyntax typeDecl)
                return false;

            if (typeDecl.AttributeLists.Count == 0)
                return false;

            if (context.SemanticModel.GetDeclaredSymbol(typeDecl) is not INamedTypeSymbol typeSymbol)
                return false;

            if (typeSymbol.Interfaces.Length == 0)
                return false;

            foreach (var attribute in typeDecl.AttributeLists.SelectMany(a => a.Attributes))
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is IMethodSymbol methodSymbol)
                {
                    var implFullName = typeSymbol.ToDisplayString();
                    var firstInterface = typeSymbol.Interfaces.FirstOrDefault()?.ToDisplayString();
                    syntaxInfo = new LibraryInfo(implFullName, firstInterface ?? string.Empty);
                    return true;
                }
            }
            syntaxInfo = default;
            return false;
        }
    }
}