using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Asprtu.Gens.Filters;

public sealed class ClassWithAnyInterface : ISyntaxFilter
{
    private ClassWithAnyInterface()
    { }

    public static ClassWithAnyInterface Instance { get; } = new();

    public bool IsMatch(SyntaxNode node)
    {
        // 只处理类
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        // 没有继承/实现任何东西，直接跳过
        if (classDecl.BaseList == null)
            return false;
        foreach (var baseType in classDecl.BaseList.Types)
        {
            return true;
        }

        return false;
    }
}