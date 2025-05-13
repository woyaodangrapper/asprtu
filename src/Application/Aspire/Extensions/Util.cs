using System.Text.RegularExpressions;

namespace Aspire.Extensions;

internal static class Util
{
    internal static string ToKebabCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        // 使用正则在大写前插入 -
        string result = Regex.Replace(input, "(?<!^)([A-Z])", "-$1");

        return result.ToLowerInvariant();
    }
}
