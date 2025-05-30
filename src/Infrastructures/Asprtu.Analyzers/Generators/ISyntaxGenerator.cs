using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using resource_analyzers.Models;
using System;
using System.Collections.Immutable;

namespace resource_analyzers.Generators;

public interface ISyntaxGenerator
{
    void Generate(
        SourceProductionContext context,
        string assemblyName,
        ImmutableArray<SyntaxInfo> syntaxInfos,
        Action<string, SourceText> addSource);
}