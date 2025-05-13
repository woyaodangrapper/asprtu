using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class ExceptionHandlerExtensions
{
    /// <summary>
    /// 启用 异常处理程序和状态码页面。
    /// </summary>
    /// <param name="app">Web 应用程序实例。</param>
    public static WebApplication UseExceptionHandler(this WebApplication app)
    {
        _ = !app.Environment.IsDevelopment() ? app.UseExceptionHandler() : app.UseDeveloperExceptionPage();
        _ = app.UseStatusCodePages();
        _ = app.UseHsts();

        return app;
    }


}
