using Asprtu.Capacities.EventHub.Mqtt.Messages;
using System.Text.Json.Serialization;

namespace Asprtu.Capacities.EventHub.Mqtt;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$case")]
public abstract class MqttAbstractMessage
{
}

[JsonSourceGenerationOptions(WriteIndented = false, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(HellMessage))]
public partial class MqttJsonContext : JsonSerializerContext
{
}