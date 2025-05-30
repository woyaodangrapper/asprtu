using Microsoft.CodeAnalysis;

namespace resource_analyzers;

public static class Errors
{
    public static readonly DiagnosticDescriptor KeyParameterMissing =
     new(
         id: "HC0074",
         title: "关键参数缺失",
         messageFormat: "关键参数缺失，请检查方法签名。",
         category: "DataLoader",
         DiagnosticSeverity.Error,
         isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MethodAccessModifierInvalid =
        new(
            id: "HC0075",
            title: "访问修饰符无效",
            messageFormat: "方法访问修饰符无效，请使用 public。",
            category: "DataLoader",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObjectTypePartialKeywordMissing =
        new(
            id: "HC0080",
            title: "缺少 partial 关键字",
            messageFormat: "拆分的对象类型类必须是 partial 类。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObjectTypeStaticKeywordMissing =
        new(
            id: "HC0081",
            title: "缺少 static 关键字",
            messageFormat: "拆分的对象类型类必须是 static 类。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InterfaceTypePartialKeywordMissing =
        new(
            id: "HC0082",
            title: "缺少 partial 关键字",
            messageFormat: "拆分的接口类型类必须是 partial 类。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InterfaceTypeStaticKeywordMissing =
        new(
            id: "HC0083",
            title: "缺少 static 关键字",
            messageFormat: "拆分的接口类型类必须是 static 类。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TooManyNodeResolverArguments =
        new(
            id: "HC0084",
            title: "参数过多",
            messageFormat: "节点解析器只能有一个名为 `id` 的字段参数。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidNodeResolverArgumentName =
        new(
            id: "HC0085",
            title: "无效的参数名称",
            messageFormat: "节点解析器只能有一个名为 `id` 的字段参数。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DataLoaderCannotBeGeneric =
        new(
            id: "HC0086",
            title: "DataLoader 不能是泛型",
            messageFormat: "DataLoader 源生成器无法生成泛型 DataLoader。",
            category: "DataLoader",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConnectionSingleGenericTypeArgument =
        new(
            id: "HC0087",
            title: "无效的连接结构",
            messageFormat: "泛型连接/边类型必须只有一个泛型参数，表示节点类型。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConnectionNameFormatIsInvalid =
        new(
            id: "HC0088",
            title: "连接/边名称格式无效",
            messageFormat: "连接/边名称必须符合 `{0}Edge` 或 `{0}Connection` 格式。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConnectionNameDuplicate =
        new(
            id: "HC0089",
            title: "连接/边名称重复",
            messageFormat: "类型 `{0}` 不能映射为 GraphQL 类型名 `{1}`，因为 `{2}` 已经映射到该名称。",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
}