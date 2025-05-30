using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Asprtu.Gens.Models;
using System;
using System.Collections.Immutable;

namespace Asprtu.Gens.Generators;

public interface ISyntaxGenerator
{
    void Generate(
        SourceProductionContext context,
        string assemblyName,
        ImmutableArray<SyntaxInfo> syntaxInfos,
        Action<string, SourceText> addSource);
}