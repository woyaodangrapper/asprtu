using Microsoft.Extensions.Logging;

namespace Asprtu.Capacities.EventHub.Mqtt.Helpers;

internal static class Util
{
    /// <summary>
    /// 将逗号分隔的 "协议://主机:端口[/路径]" 字符串解析为 (Type, Host, Port, Path?) 形式的元组序列。
    /// </summary>
    /// <param name="hostList">格式为 "协议://主机:端口[/路径],…" 的字符串。</param>
    /// <returns>返回 (Type, Host, Port, Path?) 元组集合。</returns>
    /// <exception cref="FormatException">当格式非法或缺失协议/主机/端口时抛出。</exception>
    public static IEnumerable<(Uri Uri, string Type, string Host, int Port, string? Path)> ToMqttEndpoints(string hostList)
    {
        IEnumerable<(Uri Uri, string Type, string Host, int Port, string? Path)> endpoints = hostList is null
            ? throw new ArgumentNullException(nameof(hostList))
            : hostList
               .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
               .Select(entry =>
               {
                   if (!Uri.TryCreate(entry, UriKind.Absolute, out Uri? uri))
                   {
                       throw new FormatException($"无效的 URI 格式：'{entry}'");
                   }
                   if (string.IsNullOrWhiteSpace(uri.Scheme))
                   {
                       throw new FormatException($"缺失协议部分：'{entry}'");
                   }
                   if (string.IsNullOrWhiteSpace(uri.Host))
                   {
                       throw new FormatException($"无效主机格式：'{entry}'");
                   }
                   if (uri.Port <= 0)
                   {
                       throw new FormatException($"必须携带端口且端口必须大于 0：'{entry}'");
                   }
                   string? path = uri.AbsolutePath switch
                   {
                       "/" or "" => null,
                       var p => p
                   };

#pragma warning disable IDE0037 // 不推断，这样挺整洁的
                   return (
                       Uri: uri, // 完整的 URI 对象
                       Type: uri.Scheme,   // 比如 "tcp"、"wss"、"udp" 等
                       Host: uri.Host,     // 比如 "broker.example.com"
                       Port: uri.Port,     // 比如 8084
                       Path: path          // 比如 "/mqtt"，或 null
                   );
#pragma warning restore IDE0037 // 不推断，这样挺整洁的
               });

        return endpoints;
    }

    public static readonly Action<ILogger, string, Exception?> LogInformationMessage =
        LoggerMessage.Define<string>(
            LogLevel.Information, new EventId(1, nameof(MqttConnection)),
         "{Name}");

    public static readonly Action<ILogger, string, Exception?> LogErrorMessage =
        LoggerMessage.Define<string>(
            LogLevel.Warning, new EventId(1, nameof(MqttConnection)),
        "{Name}");

    public static readonly Action<ILogger, string, Exception?> LogWarningMessage =
        LoggerMessage.Define<string>(
            LogLevel.Warning, new EventId(1, nameof(MqttConnection)),
        "{Name}");
}