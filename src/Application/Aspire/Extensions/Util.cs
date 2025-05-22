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

    internal static Dictionary<string, string> Parse(string connectionString)
    {
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);
        string[] pairs = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in pairs)
        {
            string[] parts = pair.Split('=', 2); // 限定最多分割成2部分，防止值中含有'='导致异常
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                string value = parts[1].Trim();
                result[key] = value;
            }
        }

        return result;
    }
}