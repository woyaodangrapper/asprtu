using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using resource_analyzers.Builders;
using resource_analyzers.Models;
using System;
using System.Collections.Immutable;

namespace resource_analyzers.Generators;

public sealed class LibraryLoaderSyntaxGenerator : ISyntaxGenerator
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
        using var generator = new ModuleFileBuilder("LibraryList", "Microsoft.Extensions.DependencyInjection");
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
            if (syntaxInfo is not LibraryInfo library)
            {
                continue;
            }
            generator.WriteRegisterEnumerableLoaderGroup(library.InterfaceName);
        }

        generator.WriteEndRegistrationMethod();
        generator.WriteEndClass();
        generator.WriteEndNamespace();
        //System.Diagnostics.Debugger.Launch();

        addSource("LibraryExtensionsLoaderModule.Hash.g.cs", generator.ToSourceText());
    }
}