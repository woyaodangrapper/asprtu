using Microsoft.Extensions.Logging;

namespace Asprtu.Core.Logging;

internal static class LogEvents
{
    // 基础 CRUD 操作
    internal static readonly EventId Create = new(1000, "Create");

    internal static readonly EventId Read = new(1001, "Read");
    internal static readonly EventId Update = new(1002, "Update");
    internal static readonly EventId Delete = new(1003, "Delete");

    // 扩展的常用操作
    internal static readonly EventId List = new(1004, "List");

    internal static readonly EventId Search = new(1005, "Search");
    internal static readonly EventId Validate = new(1006, "Validate");
    internal static readonly EventId Authenticate = new(1007, "Authenticate");
    internal static readonly EventId Authorize = new(1008, "Authorize");
    internal static readonly EventId Export = new(1009, "Export");
    internal static readonly EventId Import = new(1010, "Import");

    // 错误和异常事件
    internal static readonly EventId ValidationError = new(2000, "ValidationError");

    internal static readonly EventId AuthenticationError = new(2001, "AuthenticationError");
    internal static readonly EventId AuthorizationError = new(2002, "AuthorizationError");
    internal static readonly EventId NotFound = new(2003, "NotFound");
    internal static readonly EventId Conflict = new(2004, "Conflict");
    internal static readonly EventId Timeout = new(2005, "Timeout");
    internal static readonly EventId Exception = new(2006, "Exception");
    internal static readonly EventId CriticalError = new(2007, "CriticalError");

    // 系统事件
    internal static readonly EventId SystemStart = new(3000, "SystemStart");

    internal static readonly EventId SystemStop = new(3001, "SystemStop");
    internal static readonly EventId HealthCheck = new(3002, "HealthCheck");
    internal static readonly EventId ConfigurationLoad = new(3003, "ConfigurationLoad");
    internal static readonly EventId ConfigurationError = new(3004, "ConfigurationError");
}