using System;

namespace Asprtu.Gens.Helpers;

public static class CodeWriterExtensions
{
    public static void WriteGeneratedAttribute(this CodeWriter writer)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }
    }

    public static void WriteFileHeader(this CodeWriter writer)
    {
        if (writer is null)
        {
            throw new ArgumentNullException(nameof(writer));
        }

        writer.WriteIndentedLine("// <auto-generated/>");
        writer.WriteLine();
        writer.WriteIndentedLine("#nullable enable");
        writer.WriteIndentedLine("#pragma warning disable");
        writer.WriteLine();
    }

    public static CodeWriter WriteComment(this CodeWriter writer, string comment)
    {
        writer.Write("// ");
        writer.WriteLine(comment);
        return writer;
    }
}
