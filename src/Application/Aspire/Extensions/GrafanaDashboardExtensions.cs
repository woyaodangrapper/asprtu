namespace Aspire.Extensions;

public static class GrafanaDashboardExtensions
{
    public static IResourceBuilder<ContainerResource> AddGrafana(this IDistributedApplicationBuilder builder)
    {
        IResourceBuilder<ContainerResource> prometheus = builder.WithPrometheusExporter();
        IResourceBuilder<ContainerResource> grafana = builder.WithGrafanaDashboard();

        return grafana = grafana.WaitFor(prometheus);
    }

    public static IResourceBuilder<ContainerResource> WithPrometheusExporter(this IDistributedApplicationBuilder builder, int scrapePort = 9090)
    {
        IResourceBuilder<ContainerResource> container = builder.AddContainer("prometheus", "prom/prometheus:latest")

              .WithHttpEndpoint(name: "http", port: scrapePort, targetPort: 9090);
        container = container
        .WithBindMount(source: "./Resources/prometheus/prometheus.yml", target: "/etc/prometheus/prometheus.yml");
        container = container.WithArgs(
           "--config.file=/etc/prometheus/prometheus.yml",
           "--storage.tsdb.retention.time=30d");//# 数据保存 30 天，适合长期趋势分析

        return container;
    }

    public static IResourceBuilder<ContainerResource> WithGrafanaDashboard(this IDistributedApplicationBuilder builder, int dashboardPort = 3000)
    {
        IResourceBuilder<ContainerResource> container = builder.AddContainer("grafana", "grafana/grafana:latest")
                .WithHttpEndpoint(name: "http", port: dashboardPort, targetPort: 3000);

        container = container
        .WithEnvironment("GF_SECURITY_ADMIN_USER", "admin")
        .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", "public")
        .WithEnvironment("GF_PROVISIONING_PATH", "/etc/grafana/provisioning")
        .WithEnvironment("GF_DASHBOARDS_DEFAULT_HOME_DASHBOARD_PATH", "/etc/grafana/provisioning/dashboards/mqtt-nanomq-overview.json");

        container = container
            .WithBindMount("./Resources/grafana/provisioning/", "/etc/grafana/provisioning/")
            .WithBindMount("./Resources/data/grafana", "/var/lib/grafana");

        return container;
    }
}