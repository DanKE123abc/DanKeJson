using System.Text;

namespace DanKeJson.Utils
{
    /// <summary>
    /// CommentParser : 移除 JSON5 文本中的注释（// 与 /* */），字符串字面量内部的内容保持不变。
    /// </summary>
    public static class CommentParser
    {
        /// <summary>
        /// RemoveComments
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string RemoveComments(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return json;
            }

            StringBuilder builder = new StringBuilder(json.Length);
            for (int i = 0; i < json.Length; i++)
            {
                char current = json[i];

                // 字符串字面量：原样复制，注释符号在字符串内不生效
                if (current == '"' || current == '\'')
                {
                    builder.Append(current);
                    i++;
                    while (i < json.Length)
                    {
                        char inner = json[i];
                        builder.Append(inner);

                        if (inner == '\\' && i + 1 < json.Length)
                        {
                            builder.Append(json[i + 1]);
                            i += 2;
                            continue;
                        }

                        i++;
                        if (inner == current)
                        {
                            break;
                        }
                    }

                    i--;
                    continue;
                }

                if (current == '/' && i + 1 < json.Length)
                {
                    char next = json[i + 1];

                    // 行注释：保留行尾换行符
                    if (next == '/')
                    {
                        i += 2;
                        while (i < json.Length && json[i] != '\n' && json[i] != '\r')
                        {
                            i++;
                        }

                        i--;
                        continue;
                    }

                    // 块注释
                    if (next == '*')
                    {
                        i += 2;
                        while (i + 1 < json.Length && !(json[i] == '*' && json[i + 1] == '/'))
                        {
                            i++;
                        }

                        i++;
                        continue;
                    }
                }

                builder.Append(current);
            }

            return builder.ToString();
        }
    }
}
