using Microsoft.CodeAnalysis;

namespace resource_analyzers.Filters;

public interface ISyntaxFilter
{
    bool IsMatch(SyntaxNode node);
}