using Asprtu.Gens.Filters;
using Asprtu.Gens.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Asprtu.Gens.Inspectors;

/// <summary>
/// The syntax inspector will analyze a syntax node and try to reason out the semantics in a
/// Hot Chocolate server context.
/// </summary>
public interface ISyntaxInspector
{
    /// <summary>
    /// Gets the filters that is used to determine in what kinds of syntax nodes the inspector is interested.
    /// </summary>
    ImmutableArray<ISyntaxFilter> Filters { get; }

    /// <summary>
    /// Gets the kinds of syntax nodes that the inspector is interested in.
    /// </summary>
    IImmutableSet<SyntaxKind> SupportedKinds { get; }

    /// <summary>
    /// <para>
    /// Inspects the current syntax node and if the current inspector can handle
    /// the syntax will produce a syntax info.
    /// </para>
    /// <para>
    /// The syntax info is used by a syntax generator to produce source code.
    /// </para>
    /// </summary>
    bool TryHandle(
        GeneratorSyntaxContext context,
        [NotNullWhen(true)] out SyntaxInfo? syntaxInfo);
}