using Microsoft.CodeAnalysis;

namespace resource_analyzers;

public static class Errors
{
    public static readonly DiagnosticDescriptor KeyParameterMissing =
     new(
         id: "HC0074",
         title: "�ؼ�����ȱʧ",
         messageFormat: "�ؼ�����ȱʧ�����鷽��ǩ����",
         category: "DataLoader",
         DiagnosticSeverity.Error,
         isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MethodAccessModifierInvalid =
        new(
            id: "HC0075",
            title: "�������η���Ч",
            messageFormat: "�����������η���Ч����ʹ�� public��",
            category: "DataLoader",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObjectTypePartialKeywordMissing =
        new(
            id: "HC0080",
            title: "ȱ�� partial �ؼ���",
            messageFormat: "��ֵĶ�������������� partial �ࡣ",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ObjectTypeStaticKeywordMissing =
        new(
            id: "HC0081",
            title: "ȱ�� static �ؼ���",
            messageFormat: "��ֵĶ�������������� static �ࡣ",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InterfaceTypePartialKeywordMissing =
        new(
            id: "HC0082",
            title: "ȱ�� partial �ؼ���",
            messageFormat: "��ֵĽӿ������������ partial �ࡣ",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InterfaceTypeStaticKeywordMissing =
        new(
            id: "HC0083",
            title: "ȱ�� static �ؼ���",
            messageFormat: "��ֵĽӿ������������ static �ࡣ",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TooManyNodeResolverArguments =
        new(
            id: "HC0084",
            title: "��������",
            messageFormat: "�ڵ������ֻ����һ����Ϊ `id` ���ֶβ�����",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor InvalidNodeResolverArgumentName =
        new(
            id: "HC0085",
            title: "��Ч�Ĳ�������",
            messageFormat: "�ڵ������ֻ����һ����Ϊ `id` ���ֶβ�����",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor DataLoaderCannotBeGeneric =
        new(
            id: "HC0086",
            title: "DataLoader �����Ƿ���",
            messageFormat: "DataLoader Դ�������޷����ɷ��� DataLoader��",
            category: "DataLoader",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConnectionSingleGenericTypeArgument =
        new(
            id: "HC0087",
            title: "��Ч�����ӽṹ",
            messageFormat: "��������/�����ͱ���ֻ��һ�����Ͳ�������ʾ�ڵ����͡�",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConnectionNameFormatIsInvalid =
        new(
            id: "HC0088",
            title: "����/�����Ƹ�ʽ��Ч",
            messageFormat: "����/�����Ʊ������ `{0}Edge` �� `{0}Connection` ��ʽ��",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ConnectionNameDuplicate =
        new(
            id: "HC0089",
            title: "����/�������ظ�",
            messageFormat: "���� `{0}` ����ӳ��Ϊ GraphQL ������ `{1}`����Ϊ `{2}` �Ѿ�ӳ�䵽�����ơ�",
            category: "TypeSystem",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
}