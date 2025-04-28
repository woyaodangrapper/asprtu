using System.Reflection;
using Xunit.Sdk;

namespace Asprtu.Repository.MemoryCache.Tests;

public sealed class RepeatAttribute(int count) : DataAttribute
{
    public int Count { get; } = count;

    public override IEnumerable<object[]> GetData(MethodInfo testMethod) => Enumerable
        .Range(1, Count)
        .Select(i => new object[] { i });
}