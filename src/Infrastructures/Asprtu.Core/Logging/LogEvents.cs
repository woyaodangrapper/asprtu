using Microsoft.Extensions.Logging;

namespace Asprtu.Core.Logging;

public static class LogEvents
{
    // 基础 CRUD 操作
    public static readonly EventId Create = new(1000, "Create");

    public static readonly EventId Read = new(1001, "Read");
    public static readonly EventId Update = new(1002, "Update");
    public static readonly EventId Delete = new(1003, "Delete");

    // 扩展的常用操作
    public static readonly EventId List = new(1004, "List");

    public static readonly EventId Search = new(1005, "Search");
    public static readonly EventId Validate = new(1006, "Validate");
    public static readonly EventId Authenticate = new(1007, "Authenticate");
    public static readonly EventId Authorize = new(1008, "Authorize");
    public static readonly EventId Export = new(1009, "Export");
    public static readonly EventId Import = new(1010, "Import");

    // 错误和异常事件
    public static readonly EventId ValidationError = new(2000, "ValidationError");

    public static readonly EventId AuthenticationError = new(2001, "AuthenticationError");
    public static readonly EventId AuthorizationError = new(2002, "AuthorizationError");
    public static readonly EventId NotFound = new(2003, "NotFound");
    public static readonly EventId Conflict = new(2004, "Conflict");
    public static readonly EventId Timeout = new(2005, "Timeout");
    public static readonly EventId Exception = new(2006, "Exception");
    public static readonly EventId CriticalError = new(2007, "CriticalError");

    // 系统事件
    public static readonly EventId SystemStart = new(3000, "SystemStart");

    public static readonly EventId SystemStop = new(3001, "SystemStop");
    public static readonly EventId HealthCheck = new(3002, "HealthCheck");
    public static readonly EventId ConfigurationLoad = new(3003, "ConfigurationLoad");
    public static readonly EventId ConfigurationError = new(3004, "ConfigurationError");

    // Added missing definitions
    public static readonly EventId ContainerValidationError = new(4000, "ContainerValidationError");

    public static readonly EventId ContainerSystemStart = new(4001, "ContainerSystemStart");
    public static readonly EventId ContainerException = new(4002, "ContainerException");
    public static readonly EventId ContainerCriticalError = new(4003, "ContainerCriticalError");
}