using Asprtu.Capacities.Responses;
using System.Text.Json.Serialization;

namespace Asprtu.Capacities;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$case")]
public abstract class AppAbstractMessage
{
}

[JsonSourceGenerationOptions(WriteIndented = false, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(TcpResponse))]
[JsonSerializable(typeof(TcpResponse[]))]
public partial class AppJsonSerializerContext : JsonSerializerContext
{
}