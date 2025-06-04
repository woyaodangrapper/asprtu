using Asprtu.Gens.Filters;
using Asprtu.Gens.Generators;
using Asprtu.Gens.Helpers;
using Asprtu.Gens.Inspectors;
using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Asprtu.Gens;

#pragma warning disable RS1041 // 无所谓

[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "Asprtu.Rtu.Attributes.LibraryCapacitiesAttribute";

    static SourceGenerator()
    {
        //System.Diagnostics.Debugger.Launch();

        var inspectorLookup = new Dictionary<SyntaxKind, List<ISyntaxInspector>>();
        var filterBuilder = new SyntaxFilterBuilder();
        foreach (var inspector in _allInspectors)
        {
            filterBuilder.AddRange(inspector.Filters);
            foreach (var supportedKind in inspector.SupportedKinds)
            {
                if (!inspectorLookup.TryGetValue(supportedKind, out var inspectors))
                {
                    inspectors = [];
                    inspectorLookup[supportedKind] = inspectors;
                }

                inspectors.Add(inspector);
            }
        }
        _predicate = filterBuilder.Build();
        _inspectorLookup = inspectorLookup.ToFrozenDictionary(t => t.Key, t => t.Value.ToImmutableArray());
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxInfos = context.SyntaxProvider
        .CreateSyntaxProvider(static (s, _) => Predicate(s),
                              static (ctx, _) => Transform(ctx))
        .NotNull()
        .Collect();

        var postProcessedSyntaxInfos = context.CompilationProvider
            .Combine(syntaxInfos)
            .Select(static (ctx, _) => OnAfterCollect(ctx.Left, ctx.Right));

        var assemblyNameProvider = context.CompilationProvider
            .Select(static (c, _) => c.AssemblyName!);

        var valueProvider = assemblyNameProvider.Combine(postProcessedSyntaxInfos);
        context.RegisterSourceOutput(
           valueProvider, (context, source) => Execute(context, source.Left, source.Right));
    }

    // 不同语法节点对应的检查器，用来快速找到谁来处理某个节点
    private static readonly FrozenDictionary<SyntaxKind, ImmutableArray<ISyntaxInspector>> _inspectorLookup;

    // 判断一个节点是不是我们关心的，比如有没有加特定的特性
    private static readonly Func<SyntaxNode, bool> _predicate;

    // 所有的代码生成器，用来最后生成代码
    private static readonly ISyntaxGenerator[] _generators = [
          new LibraryLoaderSyntaxGenerator(),
          //new LibraryFactoriesLoaderSyntaxGenerator()
    ];

    // 收集完语法信息后用的后处理器，比如跨项目找特性的类
    private static readonly IPostCollectSyntaxTransformer[] _postCollectTransformers = [
        new AttributedCrossClassTransformer(AttributeFullName)
    ];

    // 所有的语法检查器，比如用来找加了某个特性的类
    private static readonly ISyntaxInspector[] _allInspectors = [
        new LibraryAttributeInspector(),
        //new LibraryFactoriesInspector(),
    ];

    private static void Execute(
       SourceProductionContext context,
       string assemblyName,
       ImmutableArray<SyntaxInfo> syntaxInfos)
    {
        var processedFiles = PooledObjects.GetStringSet();
        try
        {
            foreach (var syntaxInfo in syntaxInfos.AsSpan())
            {
                if (syntaxInfo.Diagnostics.Length > 0)
                {
                    foreach (var diagnostic in syntaxInfo.Diagnostics.AsSpan())
                    {
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }

            foreach (var generator in _generators.AsSpan())
            {
                generator.Generate(context, assemblyName, syntaxInfos, AddSource);
            }
        }
        finally
        {
            PooledObjects.Return(processedFiles);
        }

        void AddSource(string fileName, SourceText sourceText)
        {
            // 是
            if (processedFiles.Add(fileName))
            {
                context.AddSource(fileName, sourceText);
            }
        }
    }

    private static bool Predicate(SyntaxNode node) => _predicate(node);

    private static SyntaxInfo? Transform(GeneratorSyntaxContext context)
    {
        if (!_inspectorLookup.TryGetValue(context.Node.Kind(), out var inspectors))
        {
            return null;
        }

        foreach (var inspector in inspectors)
        {
            if (inspector.TryHandle(context, out var syntaxInfo))
            {
                return syntaxInfo;
            }
        }

        return null;
    }

    private static ImmutableArray<SyntaxInfo> OnAfterCollect(
        Compilation compilation,
        ImmutableArray<SyntaxInfo> syntaxInfos)
    {
        foreach (var transformer in _postCollectTransformers)
        {
            syntaxInfos = transformer.Transform(compilation, syntaxInfos);
        }

        return syntaxInfos;
    }
}

public sealed class AttributeSyntaxInfo(string libraryName) : SyntaxInfo
{
    public string LibraryName { get; } = libraryName;

    public override string OrderByKey => LibraryName;

    public override bool Equals(SyntaxInfo? other)
        => other is AttributeSyntaxInfo o && o.LibraryName == LibraryName;

    public override int GetHashCode() => LibraryName.GetHashCode();
}

internal static class Extensions
{
    public static IncrementalValuesProvider<SyntaxInfo> NotNull(
        this IncrementalValuesProvider<SyntaxInfo?> source)
        => source.Where(static t => t is not null)!;
}