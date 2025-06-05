using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Asprtu.Capacities.Host.Extensions;

internal static class ApplicationLaunchSettings
{
    /// <summary>
    /// 从指定资源路径加载并合并项目的 launchSettings.json 配置到应用配置中。
    /// </summary>
    /// <typeparam name="TBuilder">继承自 <see cref="IHostApplicationBuilder"/> 的宿主构建器类型。</typeparam>
    /// <param name="builder">宿主应用构建器实例。</param>
    /// <param name="resourceName">
    /// 可选参数，指定项目资源名称，用于定位 launchSettings.json 文件的父目录。
    /// 如果为 <c>null</c>，则直接使用项目源路径。
    /// </param>
    /// <returns>返回同一构建器实例，支持链式调用。</returns>
    /// <exception cref="InvalidOperationException">当项目源路径未设置时抛出。</exception>
    /// <exception cref="FileNotFoundException">
    /// 当找不到指定路径下的 launchSettings.json 文件时抛出，
    /// 并包含详细异常信息。
    /// </exception>
    internal static TBuilder AddLaunchSettings<TBuilder>(this TBuilder builder, string? resourceName = null)
        where TBuilder : IHostApplicationBuilder
    {
        string? launchProfilePath = ProjectSourcePath.Value;

        if (string.IsNullOrEmpty(launchProfilePath))
        {
            throw new InvalidOperationException("Content root path is not set.");
        }

        string basePath =
           resourceName is null ? launchProfilePath : (Directory.GetParent(launchProfilePath)?.FullName ?? launchProfilePath) + resourceName;
        string launchSettingsFilePath = System.IO.Path.Combine(basePath, "Properties", "launchSettings.json");

        try
        {
            if (!File.Exists(launchSettingsFilePath))
            {
                throw new FileNotFoundException($"The launch settings file '{launchSettingsFilePath}' does not exist.");
            }

            IConfigurationRoot launchSettings = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("Properties/launchSettings.json", optional: false)
                .Build();
            _ = builder.Configuration.AddConfiguration(launchSettings);
            return builder;
        }
        catch (FileNotFoundException ex)
        {
            string message = $"Failed to get effective launch profile for project resource '{resourceName}'. An unexpected error occurred while loading the launch settings file '{launchSettingsFilePath}': {ex.Message}";
            throw new FileNotFoundException(message, ex);
        }
    }

    /// <summary>
    /// 为宿主构建器添加 <c>appsettings.json</c> 和 <c>appsettings.{Environment}.json</c> 配置文件，以及环境变量配置。
    /// 支持热加载和环境分层配置。
    /// </summary>
    /// <typeparam name="TBuilder">继承自 <see cref="IHostApplicationBuilder"/> 的宿主构建器类型。</typeparam>
    /// <param name="builder">宿主应用构建器实例。</param>
    /// <returns>返回同一构建器实例，支持链式调用。</returns>
    internal static TBuilder AddAppsettings<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        _ = builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
             .AddJsonFile(
             $"appsettings.{builder.Environment.EnvironmentName}.json",
             optional: true, reloadOnChange: true)
             .AddEnvironmentVariables();

        return builder;
    }

    /// <summary>
    /// 配置 <see cref="IWebHostBuilder"/> 使用 Kestrel 服务器，
    /// 并根据 <c>DOTNET_LAUNCH_PROFILE</c> 以及配置文件中的 profiles 自动设置监听地址和 HTTPS 支持。
    /// </summary>
    /// <typeparam name="TBuilder">继承自 <see cref="IWebHostBuilder"/> 的 Web 主机构建器类型。</typeparam>
    /// <param name="builder">Web 主机构建器实例。</param>
    /// <returns>返回同一构建器实例，支持链式调用。</returns>
    internal static TBuilder UseKestrel<TBuilder>(this TBuilder builder)
       where TBuilder : IWebHostBuilder
    {
        _ = builder.ConfigureServices((context, services) =>
        {
            IConfiguration configuration = context.Configuration;
            // 该环境变量会在应用程序启动时自动设置，其值为当前选定的启动配置文件（launch profile）的名称。
            // 但实际为空，如果官方修复了，会先使用该环境变量的值。（如果通过 Aspire 启动则有值）
            string profile = Environment.GetEnvironmentVariable("DOTNET_LAUNCH_PROFILE")
                ?? configuration["DOTNET_LAUNCH_PROFILE"] ?? "default";

            string? httpUrl = configuration["profiles:http:applicationUrl"];
            string? httpsUrl = configuration["profiles:https:applicationUrl"];
            string? profileUrl = configuration[$"profiles:{profile}:applicationUrl"];
            string urls = profileUrl ?? ((httpsUrl ?? httpUrl) ?? configuration["urls"])
                       ?? "http://localhost:5000"; // 默认地址

            _ = builder.UseUrls(urls);

            if (urls.Contains("https://", StringComparison.OrdinalIgnoreCase))
            {
                _ = builder.UseKestrelHttpsConfiguration(); // 自动启用 https
            }
        }).ConfigureKestrel((context, options) => options.Configure(context.Configuration.GetSection("Kestrel")));
        return builder;
    }

    ///<summary>
    ///Provides the full path to the source directory of the current project.<br/>
    ///(Only meaningful on the machine where this code was compiled.)<br/>
    ///From <a href="https://stackoverflow.com/a/66285728/773113"/>
    ///</summary>
    internal static class ProjectSourcePath
    {
        ///<summary>
        ///The full path to the source directory of the current project.
        ///</summary>
        public static string? Value => field ??= Calculate();

        private static string? Calculate([System.Runtime.CompilerServices.CallerFilePath] string? path = null)
        {
            string? appName = Assembly.GetExecutingAssembly().GetName().Name;

            if (appName is null || path is null)
            {
                return null;
            }

            DirectoryInfo dir = new(path);
            while (dir.Name != appName)
            {
                dir = Directory.GetParent(dir.FullName)!;
            }
            return dir.FullName;
        }
    }
}