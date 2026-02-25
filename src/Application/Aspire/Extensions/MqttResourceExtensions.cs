namespace Aspire.Extensions;

internal static class NanomqResourceBuilderExtensions
{
    public static IResourceBuilder<NanomqResource> AddNanomq(this IDistributedApplicationBuilder builder,
       [ResourceName] string name,
       IResourceBuilder<ParameterResource>? password = null,
       IResourceBuilder<ParameterResource>? username = null)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));
        ParameterResource passwordParameter = password?.Resource ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(builder, $"{name}-password", special: false);
        ParameterResource usernameParameter = username?.Resource ?? ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(builder, $"{name}-username", special: false);

        NanomqResource nanomq = new(name, passwordParameter, usernameParameter);
        IResourceBuilder<NanomqResource> resource = builder.AddResource(nanomq);

        resource = resource.WithImage(ContainerImageTags.Image, ContainerImageTags.Tag)
                .WithImageRegistry(ContainerImageTags.Registry)
                .WithEnvironment(context =>
                {
                    context.EnvironmentVariables["NANO_WORKER_POOL_SIZE"] = 4;//# Number of worker threads
                    context.EnvironmentVariables["NANO_MAX_CLIENTS"] = 100000;//# Max simultaneous clients
                    context.EnvironmentVariables["NANO_KEEPALIVE_THRESHOLD"] = 60;//# Keepalive check interval (seconds)
                    context.EnvironmentVariables["NANO_HB_INTERVAL"] = 30; //# Healthcheck publish interval
                });
        resource = resource.UseSystem();
        return resource;
    }

    public static IResourceBuilder<NanomqResource> WithWs(this IResourceBuilder<NanomqResource> builder, int? port)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithHttpEndpoint(
              name: NanomqResource.WebSocketEndpointName,
              port: port,
              targetPort: 8083);
    }

    public static IResourceBuilder<NanomqResource> WithTcp(this IResourceBuilder<NanomqResource> builder, int? port)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithEndpoint(
              name: NanomqResource.PrimaryEndpointName,
              port: port,
              targetPort: 1883);
    }

    public static IResourceBuilder<NanomqResource> WithConf(this IResourceBuilder<NanomqResource> builder, string path)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.WithBindMount(source: path, target: "/etc/nanomq.conf");
    }

    public static IResourceBuilder<NanomqResource> WithHttp(this IResourceBuilder<NanomqResource> builder, IResourceBuilder<ParameterResource>? password, IResourceBuilder<ParameterResource>? username, int? port)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Resource.SetPassword(password?.Resource);
        builder.Resource.SetUsername(username?.Resource);

        builder = builder.WithHttpEndpoint(
            name: NanomqResource.HttpEndpointName,
            port: port,
            targetPort: 8081);

        return builder.WithEnvironment(context =>
        {
            if (builder.Resource.PasswordParameter is { } password)
            {
                context.EnvironmentVariables["NANOMQ_HTTP_SERVER_PASSWORD"] = password;
            }

            if (builder.Resource.UsernameParameter is { } username)
            {
                context.EnvironmentVariables["NANOMQ_HTTP_SERVER_USERNAME"] = username;
            }
        });
    }

    public static IResourceBuilder<NanomqResource> UseSystem(
    this IResourceBuilder<NanomqResource> builder) => builder.WithSystem().WithSysctl();

    private static IResourceBuilder<NanomqResource> WithSystem(this IResourceBuilder<NanomqResource> builder)
    {
        return builder.WithContainerRuntimeArgs("--cpus", "4")
               .WithContainerRuntimeArgs("--memory", "8g")
               .WithContainerRuntimeArgs("--memory-swap", "8g");
    }

    private static IResourceBuilder<NanomqResource> WithSysctl(this IResourceBuilder<NanomqResource> builder)
        => builder.WithContainerRuntimeArgs("--sysctl", "net.core.somaxconn=1024").WithContainerRuntimeArgs("--sysctl", "net.ipv4.tcp_max_syn_backlog=4096");
}

public class NanomqResource(string name) : ContainerResource(name), IResourceWithConnectionString
{
    public NanomqResource(string name, ParameterResource username, ParameterResource password) : this(name)
    {
        PasswordParameter = password;
        UsernameParameter = username;
    }

    internal const string PrimaryEndpointName = "tcp";

    internal const string HttpEndpointName = "http";

    internal const string WebSocketEndpointName = "ws";

    public EndpointReference HttpEndpoint => field ??= new(this, HttpEndpointName);
    public EndpointReference PrimaryEndpoint => field ??= new(this, PrimaryEndpointName);
    public EndpointReference WebSocketEndpoint => field ??= new(this, WebSocketEndpointName);

    public ParameterResource? PasswordParameter { get; private set; }
    public ParameterResource? UsernameParameter { get; private set; }

    private ReferenceExpression BuildConnectionString()
    {
        ReferenceExpressionBuilder builder = new();
        // 只追加已分配的端点
        if (HttpEndpoint.IsAllocated)
        {
            builder.Append($"{HttpEndpoint.Property(EndpointProperty.HostAndPort)}");
        }
        if (PrimaryEndpoint.IsAllocated)
        {
            builder.Append($"mqtt://{PrimaryEndpoint.Property(EndpointProperty.HostAndPort)}");
        }
        else if (WebSocketEndpoint.IsAllocated)
        {
            builder.Append($"ws://{WebSocketEndpoint.Property(EndpointProperty.HostAndPort)}");
        }


        if (PasswordParameter is not null && UsernameParameter is not null)
        {
            builder.Append($",password={PasswordParameter},username={UsernameParameter}");
        }

        return builder.Build();
    }

    public ReferenceExpression ConnectionStringExpression
    {
        get
        {
            return this.TryGetLastAnnotation(out ConnectionStringRedirectAnnotation? connectionStringAnnotation)
                ? connectionStringAnnotation.Resource.ConnectionStringExpression
                : BuildConnectionString();
        }
    }

    public ValueTask<string?> GetConnectionStringAsync(CancellationToken cancellationToken = default)
    {
        return this.TryGetLastAnnotation(out ConnectionStringRedirectAnnotation? connectionStringAnnotation)
            ? connectionStringAnnotation.Resource.GetConnectionStringAsync(cancellationToken)
            : BuildConnectionString().GetValueAsync(cancellationToken);
    }

    internal void SetPassword(ParameterResource? password) => PasswordParameter = password;

    internal void SetUsername(ParameterResource? username) => UsernameParameter = username;
}

internal class ContainerImageTags
{
    /// <remarks>docker.io</remarks>
    public const string Registry = "docker.io";

    /// <remarks>emqx/nanomq</remarks>
    public const string Image = "emqx/nanomq";

    /// <remarks>latest</remarks>
    public const string Tag = "latest";
}

// aspire 暂不支持自定义的网络 by https://github.com/dotnet/aspire/issues/850

//string alias = string.IsNullOrWhiteSpace(options.NetworkAlias)   // 如果 NetworkAlias 为空，则让 containerName 作为默认别名
//    ? options.ContainerName
//    : options.NetworkAlias;

// 可能预期的实现是这样的：
//var net1 = builder.AddNetwork("net1");
//var net2 = builder.AddNetwork("net2");

//var app1 = builder.AddContainer("app1", "image1")
//                  .AttachTo(net1);

//var app2 = builder.AddContainer("app2", "image2")
//                  .AttachTo(net1)
//                  .AttachTo(net2);

//var app2 = builder.AddContainer("app3", "image3")
//                  .AttachTo(net2);