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

        return result.ToUpperInvariant();
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

    /// <summary>
    /// 尝试将格式为 <c>schema=host;Port=port</c> 的连接字符串解析为 <see cref="Uri"/>。
    /// </summary>
    /// <param name="str">输入字符串，格式为 schema=host;Port=port</param>
    /// <param name="uri">输出解析后的绝对 <see cref="Uri"/>，若解析失败则为 null</param>
    /// <returns>若成功解析为合法 Uri，则返回 true；否则返回 false</returns>
    internal static bool TryUriCreate(this string? str, out Uri? uri)
    {
        uri = null;
        if (str == null)
        {
            return false;
        }
        Dictionary<string, string> dict = Parse(str);
        KeyValuePair<string, string> stack = dict.First();
        if (!dict.TryGetValue("port", out string? value)
            && string.IsNullOrEmpty(value))
        {
            return false;
        }
        string url = $"{stack.Key}://{stack.Value}:{value}";
        return Uri.TryCreate(url, UriKind.Absolute, out uri);
    }
}