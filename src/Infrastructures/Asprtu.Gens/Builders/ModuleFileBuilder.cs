using Asprtu.Gens.Helpers;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Text;

namespace Asprtu.Gens.Builders;

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
            "builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton(typeof(global::Asprtu.Rtu.Contracts.ILibraryCapacities), typeof(global::Asprtu.Rtu.LibraryCapacities<>).MakeGenericType(typeof(global::{0}))));",
            interfaceTypeName);

    public void WriteRegisterSingletonLoader(string typeName, string interfaceTypeName)
    {
        _writer.WriteIndentedLine(
            "builder.Services.AddSingleton<global::Asprtu.Rtu.Contracts.ILibraryCapacities<global::{0}>, global::Asprtu.Rtu.LibraryCapacities<global::{0}>>();",
            typeName);

        //builder.Services.AddSingleton<ILibraryCapacities<ITcpServer>>(sp => sp.GetRequiredService<ILibraryCapacities<TcpServer>>());

        _writer.WriteIndentedLine(
            "builder.Services.AddSingleton<global::Asprtu.Rtu.Contracts.ILibraryCapacities>(sp => sp.GetRequiredService<global::Asprtu.Rtu.Contracts.ILibraryCapacities<global::{0}>>());",
            typeName);

        _writer.WriteIndentedLine(
            "builder.Services.AddSingleton<global::Asprtu.Rtu.Contracts.ILibraryCapacities<global::{1}>>(sp => sp.GetRequiredService<global::Asprtu.Rtu.Contracts.ILibraryCapacities<global::{0}>>());",
            typeName, interfaceTypeName);

        _writer.WriteLine();
    }

    public void WriteRegisterFactoryLoaderGroup(string interfaceName, string typeName, string typeInterfaceName)
    {
        _writer.WriteIndentedLine(
            "builder.Services.AddSingleton<global::Asprtu.Rtu.Contracts.ILibraryFactory<{1}>, {0}>();",
            interfaceName, typeName);

        _writer.WriteIndentedLine(
            "builder.Services.AddSingleton<global::Asprtu.Rtu.Contracts.ILibraryCapacitiesFactory<{0}>, global::Asprtu.Rtu.LibraryCapacitiesFactory<{0}>>();",
             typeName);

        _writer.WriteIndentedLine(
            "builder.Services.AddSingleton<global::Asprtu.Rtu.Contracts.ILibraryCapacitiesFactory<{0}>>(sp => sp.GetRequiredService<global::Asprtu.Rtu.Contracts.ILibraryCapacitiesFactory<{1}>>());",
            typeInterfaceName, typeName);
        _writer.WriteLine();
    }

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