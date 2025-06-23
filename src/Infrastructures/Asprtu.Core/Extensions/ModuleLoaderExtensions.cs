using Aspire;
using Asprtu.Core.Extensions.Module;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Asprtu.Core.Extensions;

public static class ModuleLoaderExtensions
{
    /// <summary>
    /// 从配置中加载所有模块，构建 ModuleProvider。
    /// 无法解析的模块将被忽略。
    /// </summary>
    /// <param name="config">包含模块定义的配置对象</param>
    /// <returns>包含已成功加载模块的提供器</returns>
    public static ModuleProvider TryLoad([NotNull] IConfiguration config)
    {
        IConfigurationSection modulesSection = config.GetSection("modules");

        ModuleProvider provider = new();

        foreach (IConfigurationSection section in modulesSection.GetChildren())
        {
            if (Load(section, out object? module) && module is { } model)
            {
                provider.AddDynamic(model);
            }
        }

        return provider;
    }

    /// <summary>
    /// 根据模块配置尝试构建模块实例。
    /// 支持 mqtt-server、mqtt-client 和 tcp-service。
    /// </summary>
    private static bool Load(IConfigurationSection section, out object? module)
    {
        // 预取公共字段
        string name = section.GetValue<string>("name") ?? "";
        string type = section.GetValue<string>("type") ?? "";
        bool enabled = section.GetValue<bool>("enabled");
        string image = section.GetValue<string>("image") ?? "";
        IConfigurationSection cfg = section.GetSection("config");
        string broker = cfg.GetValue<string>("host") ?? "";

        // 用 switch 表达式和 when 条件做所有分支
        module = name switch
        {
            "mqtt-server" when broker.TryCreate(out Hosts serverUri) =>
                new MqttServerModule
                {
                    Name = name,
                    Type = type,
                    Enabled = enabled,
                    Config = new MqttServerConfig(serverUri!, image, cfg.GetValue<bool>("dashboard"))
                },

            "mqtt-client" when broker.TryCreate(out Uri? clientUri) =>
                new MqttClientModule
                {
                    Name = name,
                    Type = type,
                    Enabled = enabled,
                    Config = new MqttClientConfig(clientUri!, cfg.GetValue<string>("clientId")!)
                },

            "tcp-service" when broker.TryCreate(out Uri? tcpUri) =>
                new TcpServiceModule
                {
                    Name = name,
                    Type = type,
                    Enabled = enabled,
                    Config = new TcpServiceConfig(tcpUri!)
                },

            _ => null
        };

        return module != null;
    }

    /// <summary>
    /// 尝试将格式为 <c>schema=host;Port=port</c> 的连接字符串解析为 <see cref="Uri"/>。
    /// </summary>
    /// <param name="str">输入字符串，格式为 schema=host;port=port</param>
    /// <param name="uri">输出解析后的绝对 <see cref="Uri"/>，若解析失败则为 null</param>
    /// <returns>若成功解析为合法 Uri，则返回 true；否则返回 false</returns>
    private static bool TryCreate(this string? str, out Uri? uri)
    {
        uri = null;
        if (str == null)
        {
            return false;
        }
        Dictionary<string, string> dict = Parse(str);
        KeyValuePair<string, string> stack = dict.First();
        if (!dict.TryGetValue("port", out string? value)
            && string.IsNullOrEmpty(value))
        {
            return false;
        }
        string url = $"{stack.Key}://{stack.Value}:{value}";
        return Uri.TryCreate(url, UriKind.Absolute, out uri);
    }

    /// <summary>
    /// 尝试将格式为 <c>schema=host;Port=port</c> 的连接字符串解析为 <see cref="Hosts"/>。
    /// </summary>
    /// <param name="str">输入字符串，格式为 schema=host;port=port</param>
    /// <param name="hosts">输出解析后的绝对 <see cref="Hosts"/>，若解析失败则为 null</param>
    /// <returns>若成功解析为合法 Uri，则返回 true；否则返回 false</returns>
    private static bool TryCreate(this string? str, out Hosts hosts)
    {
        if (str == null)
        {
            return false;
        }
        string[] parts = str.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (string item in parts)
        {
            if (item.TryCreate(out Uri? uri) && uri is not null)
            {
                hosts = hosts.Add(uri.Scheme, uri.Host, uri.Port);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private static Dictionary<string, string> Parse(string connectionString)
    {
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);
        string[] pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in pairs)
        {
            string[] parts = pair.Split('=', 2); // 限定最多分割成2部分，防止值中含有'='导致异常
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                result[key] = value;
            }
        }

        return result;
    }

    private class ModuleInfo
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public bool Enabled { get; set; }
    }
}

public class ModuleProvider
{
    private readonly Dictionary<Type, object> _typedModules = [];

    /// <summary>
    /// 注册一个模块，按配置类型 <typeparamref name="T"/> 进行索引。
    /// </summary>
    public void Add<T>(IModule<T> module) where T : class =>
        _typedModules[typeof(T)] = module;

    /// <summary>
    /// 动态注册模块，通过反射识别其实现的 IModule&lt;T&gt; 接口并添加。
    /// </summary>
    /// <param name="module">模块实例，必须实现 IModule&lt;T&gt;</param>
    /// <exception cref="InvalidOperationException">如果未实现 IModule&lt;&gt; 接口</exception>
    public void AddDynamic(object module)
    {
        Type? type = module?.GetType();
        Type? iface = type?.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IModule<>));

        if (iface == null || module == null)
        {
            throw new InvalidOperationException($"Object of type {type}/{module} does not implement IModule<>");
        }

        _typedModules[iface.GetGenericArguments()[0]] = module;
    }

    /// <summary>
    /// 获取指定配置类型的模块实例，未找到时抛出异常。
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <returns>实现了 IModule&lt;T&gt; 的模块实例</returns>
    /// <exception cref="InvalidOperationException">如果模块未注册</exception>
    public IModule<T> Get<T>() where T : class =>
        _typedModules.TryGetValue(typeof(T), out object? module)
            ? (IModule<T>)module
            : throw new InvalidOperationException($"Module for type {typeof(T).Name} not found.");

    /// <summary>
    /// 尝试获取指定配置类型的模块实例。
    /// </summary>
    /// <typeparam name="T">配置类型</typeparam>
    /// <param name="module">返回模块实例（如存在）</param>
    /// <returns>是否找到模块</returns>
    public bool TryGet<T>(out IModule<T>? module) where T : class
    {
        if (_typedModules.TryGetValue(typeof(T), out object? value) && value is IModule<T> typed)
        {
            module = typed;
            return true;
        }

        module = null;
        return false;
    }

    /// <summary>
    /// 获取所有已注册模块实例。
    /// </summary>
    public IEnumerable<object> All => _typedModules.Values;
}