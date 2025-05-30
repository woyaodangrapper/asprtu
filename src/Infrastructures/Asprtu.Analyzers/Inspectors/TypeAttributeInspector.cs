using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using resource_analyzers.Filters;
using resource_analyzers.Models;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace resource_analyzers.Inspectors
{
    public sealed class LibraryAttributeInspector : ISyntaxInspector
    {
        public ImmutableArray<ISyntaxFilter> Filters { get; } = [TypeWithAttribute.Instance];

        // 支持处理的节点类型：类、结构、接口、枚举
        public IImmutableSet<SyntaxKind> SupportedKinds { get; } =
        [
            SyntaxKind.ClassDeclaration,
            SyntaxKind.RecordDeclaration,
            SyntaxKind.InterfaceDeclaration,
            SyntaxKind.EnumDeclaration
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

            foreach (var attribute in typeDecl.AttributeLists.SelectMany(a => a.Attributes))
            {
                if (context.SemanticModel.GetSymbolInfo(attribute).Symbol is IMethodSymbol methodSymbol &&
                    context.SemanticModel.GetDeclaredSymbol(typeDecl) is INamedTypeSymbol typeSymbol)
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