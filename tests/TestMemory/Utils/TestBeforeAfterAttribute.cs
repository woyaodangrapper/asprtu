using System.Reflection;
using Xunit.Sdk;

namespace Asprtu.Repository.MemoryCache.Tests;

public sealed class TestBeforeAfterAttribute : BeforeAfterTestAttribute
{
    public override void Before(MethodInfo methodUnderTest) =>
        Console.WriteLine($"Before - {methodUnderTest.Name}");

    public override void After(MethodInfo methodUnderTest) =>
        Console.WriteLine($"After - {methodUnderTest.Name}");
}