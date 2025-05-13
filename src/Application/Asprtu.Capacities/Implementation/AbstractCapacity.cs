using Asprtu.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Asprtu.Capacities.Implementation;

public abstract class AbstractCapacity : IAsprtu
{
    /// <summary>
    /// 带结果的成功响应（HTTP 200），附带提示信息
    /// </summary>
    /// <typeparam name="T">返回的数据类型</typeparam>
    /// <param name="value">要返回的结果值</param>
    /// <returns>
    /// 封装的 Ok 结果。其基类为 <see cref="IResult"/>，
    /// 可被控制器或 Minimal API 使用，并兼容 AppResult 的统一响应封装策略。
    /// </returns>
    protected static Ok<T> Ok<T>(T value) => TypedResults.Ok(value);


    /// <summary>
    /// 成功响应（HTTP 200）
    /// </summary>
    /// <returns>
    /// 封装的 Ok 结果。其基类为 <see cref="IResult"/>，
    /// 可被控制器或 Minimal API 使用，并兼容 AppResult 的统一响应封装策略。
    /// </returns>
    protected static Ok Ok() => TypedResults.Ok();

    /// <summary>
    /// 返回资源未找到结果（HTTP 404），附带提示信息
    /// </summary>
    /// <typeparam name="T">返回的数据类型</typeparam>
    /// <param name="message">未找到的提示消息</param>
    /// <returns>
    /// 封装的 NotFound 结果。其基类为 <see cref="IResult"/>，
    /// 可被控制器或 Minimal API 使用，并兼容 AppResult 的统一响应封装策略。
    /// </returns>
    protected static NotFound<T> NotFound<T>(T message) => TypedResults.NotFound(message);

    /// <summary>
    /// 返回资源未找到结果（HTTP 404），无提示信息
    /// </summary>
    /// <returns>
    /// 封装的 NotFound 结果。其基类为 <see cref="IResult"/>，
    /// 可被控制器或 Minimal API 使用，并兼容 AppResult 的统一响应封装策略。
    /// </returns>
    protected static NotFound NotFound() => TypedResults.NotFound();

    /// <summary>
    /// 返回请求错误结果（HTTP 400）
    /// </summary>
    /// <typeparam name="T">返回的数据类型</typeparam>
    /// <param name="message">未找到的提示消息</param>
    /// <returns>
    /// 封装的 BadRequest 结果。其基类为 <see cref="IResult"/>，
    /// 可被控制器或 Minimal API 使用，并兼容 AppResult 的统一响应封装策略。
    /// </returns>
    protected static BadRequest<string> BadRequest(string message) => TypedResults.BadRequest(message);



    /// <summary>
    /// 返回请求错误结果（HTTP 400），无提示信息
    /// </summary>
    /// <returns>
    /// 封装的 BadRequest 结果。其基类为 <see cref="IResult"/>，
    /// 可被控制器或 Minimal API 使用，并兼容 AppResult 的统一响应封装策略。
    /// </returns>
    protected static BadRequest BadRequest() => TypedResults.BadRequest();

    /// <summary>
    /// 返回通用的错误信息（ProblemDetails），默认状态码为 500
    /// </summary>
    /// <param name="detail">错误详情</param>
    /// <param name="statusCode">HTTP 状态码</param>
    /// <returns>封装的 ProblemDetails 结果</returns>
    protected static ProblemHttpResult Problem(string? detail = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        ProblemDetails pd = new()
        {
            Title = "An error occurred",
            Detail = detail ?? "Unexpected error",
            Status = (int)statusCode
        }
        ;
        return TypedResults.Problem(pd);
    }

    /// <summary>
    /// 从异常中构建 ProblemDetails 错误信息，默认状态码为 500
    /// </summary>
    /// <param name="ex">要处理的异常实例</param>
    /// <param name="statusCode">HTTP 状态码，默认 500</param>
    /// <returns>封装的 ProblemDetails 结果</returns>
    protected static ProblemHttpResult Problem(Exception ex, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        ProblemDetails pd = new()
        {
            Title = ex.GetType().Name,
            Detail = ex.Message,
            Status = (int)statusCode,
            Extensions =
            {
                { "traceId", Guid.NewGuid().ToString("N") }, // 可选扩展字段
                { "exceptionType", ex.GetType().FullName },
            }
        };
        return TypedResults.Problem(pd);
    }

}