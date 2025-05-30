using Microsoft.CodeAnalysis.Text;
using resource_analyzers.Helpers;
using System;
using System.Text;

namespace resource_analyzers.Builders;

public sealed class ModuleFileBuilder : IDisposable
{
    private readonly string _moduleName;
    private readonly string _ns;
    private StringBuilder _sb;
    private CodeWriter _writer;
    private bool _disposed;

    public ModuleFileBuilder(string moduleName, string ns)
    {
        _moduleName = moduleName;
        _ns = ns;
        _sb = PooledObjects.GetStringBuilder();
        _writer = new CodeWriter(_sb);
    }

    public void WriteHeader(Action<CodeWriter>? action = null)
    {
        _writer.WriteFileHeader();
        if (action is not null) action(_writer);
        _writer.WriteLine();
    }

    public void WriteBeginNamespace()
    {
        _writer.WriteIndentedLine("namespace {0}", _ns);
        _writer.WriteIndentedLine("{");
        _writer.IncreaseIndent();
    }

    public void WriteEndNamespace()
    {
        _writer.DecreaseIndent();
        _writer.WriteIndentedLine("}");
    }

    public void WriteBeginClass()
    {
        _writer.WriteIndentedLine("public static partial class {0}RequestExecutorBuilderExtensions", _moduleName);
        _writer.WriteIndentedLine("{");
        _writer.IncreaseIndent();
    }

    public void WriteEndClass()
    {
        _writer.DecreaseIndent();
        _writer.WriteIndentedLine("}");
    }

    public void WriteRegisterEnumerableLoaderGroup(string interfaceTypeName)
        => _writer.WriteIndentedLine(
            "builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(ILibraryCapacities), typeof(LibraryCapacities<>).MakeGenericType(typeof(global::{0}))));",
            interfaceTypeName);

    public void WriteBeginRegistrationMethod()
    {
        _writer.WriteIndentedLine(
            "public static IHostApplicationBuilder Add{0}(this IHostApplicationBuilder builder)",
            _moduleName);
        _writer.WriteIndentedLine("{");
        _writer.IncreaseIndent();
    }

    public void WriteEndRegistrationMethod()
    {
        _writer.WriteIndentedLine("return builder;");
        _writer.DecreaseIndent();
        _writer.WriteIndentedLine("}");
    }

    public override string ToString()
        => _sb.ToString();

    public SourceText ToSourceText()
        => SourceText.From(ToString(), Encoding.UTF8);

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        PooledObjects.Return(_sb);
        _sb = default!;
        _writer = default!;
        _disposed = true;
    }
}