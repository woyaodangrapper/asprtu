using Asprtu.Core.Extensions;
using Asprtu.Core.Extensions.Module;

namespace Aspire.Contracts;

/// <summary>
/// 容器默认设置类，可通过 AddMqtt 时传入以覆盖默认值
/// </summary>
internal class MqttOptions
{
    public MqttOptions(ModuleProvider provider)
    {
        if (provider.TryGet(out IModule<MqttServerConfig>? mqtt) && mqtt != null)
        {
            Name = mqtt.Name;
            Host = mqtt.Config.Host;
            Dashboard = mqtt.Config.Dashboard;
        }
    }

    /// <summary>
    /// Gets or sets the host configuration for the application.
    /// </summary>
    public Hosts Host { get; set; }

    /// <summary>
    /// Gets or sets the name associated with the current instance.
    /// </summary>
    public string Name { get; set; } = "nanomq";

    /// <summary>
    /// 启动面板开关
    /// </summary>
    public bool Dashboard { get; set; }

    /// <summary>
    /// 用户名，默认为 "admin"
    /// </summary>
    public string Username { get; set; } = "admin";

    /// <summary>
    /// 密码，默认为 "public"
    /// </summary>
    public string Password { get; set; } = "public";
}