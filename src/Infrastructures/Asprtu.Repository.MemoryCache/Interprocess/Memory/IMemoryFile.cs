using System.IO.MemoryMappedFiles;

namespace Asprtu.Repository.MemoryCache;

internal interface IMemoryFile : IDisposable
{
    MemoryMappedFile MappedFile { get; }
}