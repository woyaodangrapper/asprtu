using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;

namespace Asprtu.Gens.Models;

public abstract class SyntaxInfo : IEquatable<SyntaxInfo>
{
    public abstract string OrderByKey { get; }

    public ImmutableArray<Diagnostic> Diagnostics { get; private set; } = ImmutableArray<Diagnostic>.Empty;

    public void AddDiagnostic(Diagnostic diagnostic)
        => Diagnostics = Diagnostics.Add(diagnostic);

    public void AddDiagnosticRange(ImmutableArray<Diagnostic> diagnostics)
        => Diagnostics = Diagnostics.IsEmpty
            ? diagnostics
            : Diagnostics.AddRange(diagnostics);

    public abstract bool Equals(SyntaxInfo? other);
}