namespace Asprtu.Capacities.Host.Endpoints;

[QueryType]
public static class TcpGraphQL
{
    public static Book GetBook() => new("C# in Depth", new Author("Jon Skeet"));
}

public record Book(string Title, Author Author);
public record Author(string Name);