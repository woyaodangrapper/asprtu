using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Asprtu.Capacities.Host.Extensions;

internal static class ApplicationLaunchSettings
{
    internal static TBuilder UseLaunchSettings<TBuilder>(this TBuilder builder, string? resourceName = null)
       where TBuilder : IHostApplicationBuilder
    {
        string? launchProfilePath = ProjectSourcePath.Value;
        if (!string.IsNullOrEmpty(builder.Configuration["DOTNET_RUNNING_IN_CONTAINER"]))
        {
            return builder;
        }

        if (string.IsNullOrEmpty(launchProfilePath))
        {
            throw new InvalidOperationException("Content root path is not set.");
        }

        string basePath =
           resourceName is null ? launchProfilePath : (Directory.GetParent(launchProfilePath)?.FullName ?? launchProfilePath) + resourceName;
        string launchSettingsFilePath = Path.Combine(basePath, "Properties", "launchSettings.json");

        try
        {
            // It isn't mandatory that the launchSettings.json file exists!
            if (!File.Exists(launchSettingsFilePath))
            {
                throw new FileNotFoundException($"The launch settings file '{launchSettingsFilePath}' does not exist.");
            }

            // 1. 读取 launchSettings.json
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

    internal static TBuilder UseKestrel<TBuilder>(this TBuilder builder)
       where TBuilder : IWebHostBuilder
    {
        // 通过builder.ConfigureServices获取IServiceCollection
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
         });

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