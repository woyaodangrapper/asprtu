using Asprtu.Gens.Builders;
using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;

namespace Asprtu.Gens.Generators;

public sealed class LibraryFactoriesLoaderSyntaxGenerator : ISyntaxGenerator
{
    public void Generate(
        SourceProductionContext context,
        string assemblyName,
        ImmutableArray<SyntaxInfo> syntaxInfos,
        Action<string, SourceText> addSource)
    {
        if (syntaxInfos.IsEmpty)
        {
            return;
        }
        using var generator = new ModuleFileBuilder("LibraryFactory", "Microsoft.Extensions.DependencyInjection");
        generator.WriteHeader((write) =>
        {
            write.WriteIndentedLine("using Microsoft.Extensions.Hosting;");
            write.WriteIndentedLine("using Microsoft.Extensions.DependencyInjection.Extensions;");
        });

        generator.WriteBeginNamespace();
        generator.WriteBeginClass();
        generator.WriteBeginRegistrationMethod();

        foreach (var syntaxInfo in syntaxInfos)
        {
            if (syntaxInfo is not FactoryInfo library)
            { continue; }

            generator.WriteRegisterFactoryLoaderGroup(library.TypeName, library.CapabilityType);
        }

        generator.WriteEndRegistrationMethod();
        generator.WriteEndClass();
        generator.WriteEndNamespace();

        addSource("LibraryFactoryExtensionsLoaderModule.Hash.g.cs", generator.ToSourceText());
    }
}