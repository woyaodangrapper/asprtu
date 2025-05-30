using Microsoft.CodeAnalysis;

namespace Asprtu.Gens.Filters;

public interface ISyntaxFilter
{
    bool IsMatch(SyntaxNode node);
}