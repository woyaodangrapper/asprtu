using Microsoft.CodeAnalysis;
using Asprtu.Gens.Models;
using System.Collections.Immutable;

namespace Asprtu.Gens.Inspectors;

/// <summary>
/// The post collect syntax transformer allows to create syntax infos based on the collected syntax infos.
/// </summary>
public interface IPostCollectSyntaxTransformer
{
    ImmutableArray<SyntaxInfo> Transform(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos);
}