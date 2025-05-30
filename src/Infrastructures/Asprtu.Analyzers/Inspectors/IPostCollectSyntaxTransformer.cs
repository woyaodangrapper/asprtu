using Microsoft.CodeAnalysis;
using resource_analyzers.Models;
using System.Collections.Immutable;

namespace resource_analyzers.Inspectors;

/// <summary>
/// The post collect syntax transformer allows to create syntax infos based on the collected syntax infos.
/// </summary>
public interface IPostCollectSyntaxTransformer
{
    ImmutableArray<SyntaxInfo> Transform(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos);
}