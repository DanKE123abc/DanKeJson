using System.Text;

namespace DanKeJson.Utils
{
    /// <summary>
    /// JsonString : 在序列化 JSON / JSON5 时使用的字符串转义辅助函数
    /// </summary>
    public static class JsonString
    {
        /// <summary>
        /// 对一个值进行转义，以便将其放入带双引号的 JSON 字符串中。
        /// </summary>
        public static string Escape(string value)
        {
            if (value == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case '"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (c < 0x20)
                        {
                            sb.Append("\\u").Append(((int)c).ToString("x4"));
                        }
                        else if (char.IsSurrogate(c))
                        {
                            // 合法代理对原样保留；孤立代理项转义，避免产出非法 JSON
                            if (char.IsHighSurrogate(c) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]))
                            {
                                sb.Append(c).Append(value[i + 1]);
                                i++;
                            }
                            else
                            {
                                sb.Append("\\u").Append(((int)c).ToString("x4"));
                            }
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 对一个值进行转义，以便将其放入单引号括起来的 JSON5 字符串中。
        /// </summary>
        public static string EscapeSingleQuote(string value)
        {
            if (value == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case '\'':
                        sb.Append("\\'");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (c < 0x20)
                        {
                            sb.Append("\\u").Append(((int)c).ToString("x4"));
                        }
                        else if (char.IsSurrogate(c))
                        {
                            // 合法代理对原样保留；孤立代理项转义，避免产出非法 JSON5
                            if (char.IsHighSurrogate(c) && i + 1 < value.Length && char.IsLowSurrogate(value[i + 1]))
                            {
                                sb.Append(c).Append(value[i + 1]);
                                i++;
                            }
                            else
                            {
                                sb.Append("\\u").Append(((int)c).ToString("x4"));
                            }
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 如果存在的话，去除存储的 JsonData 字符串字面量两端的引号。
        /// </summary>
        public static string Unquote(string literal)
        {
            if (literal != null && literal.Length >= 2 && literal[0] == '"' && literal[^1] == '"')
            {
                return literal.Substring(1, literal.Length - 2);
            }

            return literal;
        }
    }
}
