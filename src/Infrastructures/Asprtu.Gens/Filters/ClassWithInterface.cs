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
        // ֻ������
        if (node is not ClassDeclarationSyntax classDecl)
            return false;

        // û�м̳�/ʵ���κζ�����ֱ������
        if (classDecl.BaseList == null)
            return false;
        foreach (var baseType in classDecl.BaseList.Types)
        {
            return true;
        }

        return false;
    }
}