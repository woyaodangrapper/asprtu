namespace Aspire.Extensions;

internal static class MonitoringResourceBuilderExtensions
{
    public static IResourceBuilder<ContainerResource> AddMonitoring(this IDistributedApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        IResourceBuilder<ContainerResource> prometheus = builder.WithPrometheusExporter();
        IResourceBuilder<ContainerResource> grafana = builder.WithGrafanaDashboard();

        return grafana.WaitFor(prometheus);
    }

    public static IResourceBuilder<ContainerResource> WithPrometheusExporter(
        this IDistributedApplicationBuilder builder,
        int scrapePort = 9090)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        IResourceBuilder<ContainerResource> container = builder
            .AddContainer("prometheus", ContainerImageTags.PrometheusImage, ContainerImageTags.PrometheusTag)
            .WithHttpEndpoint(
                name: "http",
                port: scrapePort,
                targetPort: 9090)
            .WithBindMount(
                source: "./Resources/prometheus/prometheus.yml",
                target: "/etc/prometheus/prometheus.yml")
            .WithArgs(
                "--config.file=/etc/prometheus/prometheus.yml",
                "--storage.tsdb.retention.time=30d"); // 数据保存 30 天，适合长期趋势分析

        return container;
    }

    public static IResourceBuilder<ContainerResource> WithGrafanaDashboard(
        this IDistributedApplicationBuilder builder,
        int dashboardPort = 3000)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        IResourceBuilder<ContainerResource> container = builder
            .AddContainer("grafana", ContainerImageTags.GrafanaImage, ContainerImageTags.GrafanaTag)
            .WithHttpEndpoint(
                name: "http",
                port: dashboardPort,
                targetPort: 3000)
            .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
            .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "public")
            .WithEnvironment("GF_PROVISIONING_PATH", "/etc/grafana/provisioning")
            .WithEnvironment("GF_DASHBOARDS_DEFAULT_HOME_DASHBOARD_PATH", "/etc/grafana/provisioning/dashboards/mqtt-nanomq-overview.json")
            .WithBindMount(
                source: "./Resources/grafana/provisioning/",
                target: "/etc/grafana/provisioning/")
            .WithBindMount(
                source: "./Resources/data/grafana",
                target: "/var/lib/grafana");

        return container;
    }

    private static class ContainerImageTags
    {
        public const string PrometheusRegistry = "docker.io";
        public const string PrometheusImage = "prom/prometheus";
        public const string PrometheusTag = "latest";

        public const string GrafanaRegistry = "docker.io";
        public const string GrafanaImage = "grafana/grafana";
        public const string GrafanaTag = "latest";
    }
}