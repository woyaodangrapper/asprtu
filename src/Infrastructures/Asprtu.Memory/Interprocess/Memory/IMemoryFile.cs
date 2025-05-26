using System.IO.MemoryMappedFiles;

namespace Asprtu.Memory.Interprocess.Memory;

internal interface IMemoryFile : IDisposable
{
    MemoryMappedFile MappedFile { get; }
}