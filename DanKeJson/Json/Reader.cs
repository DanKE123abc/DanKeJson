using System.Globalization;
using System.Text;
using DanKeJson.Utils;

#pragma warning disable CS8603

namespace DanKeJson.Json
{
    public class Reader
    {
        public static void SkipWhiteSpace(string json, ref int index)
        {
            if (index < 0)
            {
                return;
            }

            while (index < json.Length && char.IsWhiteSpace(json[index]))
            {
                index++;
            }
        }
        
        public static JsonData ToString_Double(string json, ref int index)
        {
            return ToString_Double(json, ref index, false);
        }

        /// <summary>
        /// 解析双引号字符串。json5 为 true 时额外支持 \x、\v、\0、\' 转义与字符串续行。
        /// </summary>
        public static JsonData ToString_Double(string json, ref int index, bool json5)
        {
            if (index < 0 || index >= json.Length || json[index] != '\"')
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            int start = index; // 记录起始位置
            index++; // 跳过起始引号
            
            while (index < json.Length)
            {
                char current = json[index];
                
                if (current == '\"')
                {
                    index++;
                    return new JsonData(JsonData.Type.String)
                    {
                        json = "\"" + sb.ToString() + "\""
                    };
                }

                // 4. 处理转义序列
                if (current == '\\')
                {
                    index++; // 跳过反斜杠
                    if (index >= json.Length) break; // 防止越界

                    switch (json[index++]) // 处理转义字符并移动索引
                    {
                        case '\"':
                            sb.Append('\"');
                            break;
                        case '\\':
                            sb.Append('\\');
                            break;
                        case '/':
                            sb.Append('/');
                            break;
                        case 'b':
                            sb.Append('\b');
                            break;
                        case 'f':
                            sb.Append('\f');
                            break;
                        case 'n':
                            sb.Append('\n');
                            break;
                        case 'r':
                            sb.Append('\r');
                            break;
                        case 't':
                            sb.Append('\t');
                            break;
                        case 'u': // Unicode转义处理
                            if (index + 4 <= json.Length)
                            {
                                string hex = json.Substring(index, 4);
                                if (int.TryParse(hex, NumberStyles.HexNumber, null, out int code))
                                {
                                    sb.Append((char)code);
                                }

                                index += 4;
                            }

                            break;
                        // 以下转义仅在 JSON5 中生效，纯 JSON 保持原样输出
                        case 'x':
                            if (json5 && TryReadHexEscape(json, ref index, 2, out char hexValue))
                            {
                                sb.Append(hexValue);
                            }
                            else
                            {
                                AppendVerbatimEscape(sb, json, index);
                            }

                            break;
                        case 'v':
                            if (json5)
                            {
                                sb.Append('\v');
                            }
                            else
                            {
                                AppendVerbatimEscape(sb, json, index);
                            }

                            break;
                        case '0':
                            if (json5 && (index >= json.Length || !char.IsDigit(json[index])))
                            {
                                sb.Append('\0');
                            }
                            else
                            {
                                AppendVerbatimEscape(sb, json, index);
                            }

                            break;
                        case '\'':
                            if (json5)
                            {
                                sb.Append('\'');
                            }
                            else
                            {
                                AppendVerbatimEscape(sb, json, index);
                            }

                            break;
                        case '\n':
                        case '\u2028':
                        case '\u2029':
                            // JSON5 字符串续行：反斜杠与行终止符都被移除
                            if (!json5)
                            {
                                AppendVerbatimEscape(sb, json, index);
                            }

                            break;
                        case '\r':
                            if (json5)
                            {
                                if (index < json.Length && json[index] == '\n')
                                {
                                    index++;
                                }
                            }
                            else
                            {
                                AppendVerbatimEscape(sb, json, index);
                            }

                            break;
                        default: // 未知转义序列保持原样
                            AppendVerbatimEscape(sb, json, index);
                            break;
                    }
                }
                else
                {
                    // 5. 普通字符直接添加
                    sb.Append(current);
                    index++;
                }
            }

            // 6. 未找到结束引号（字符串未闭合）
            return null;
        }

        /// <summary>把未知转义序列原样写回（反斜杠 + 转义字符）。</summary>
        internal static void AppendVerbatimEscape(StringBuilder sb, string json, int index)
        {
            sb.Append('\\');
            sb.Append(json[index - 1]);
        }

        /// <summary>读取若干位十六进制数字；失败时不移动索引。</summary>
        internal static bool TryReadHexEscape(string json, ref int index, int digits, out char value)
        {
            value = default;
            if (index + digits > json.Length)
            {
                return false;
            }

            int code = 0;
            for (int i = 0; i < digits; i++)
            {
                int digit = HexValue(json[index + i]);
                if (digit < 0)
                {
                    return false;
                }

                code = (code << 4) | digit;
            }

            index += digits;
            value = (char)code;
            return true;
        }

        private static int HexValue(char c)
        {
            if (c >= '0' && c <= '9')
            {
                return c - '0';
            }

            if (c >= 'a' && c <= 'f')
            {
                return c - 'a' + 10;
            }

            if (c >= 'A' && c <= 'F')
            {
                return c - 'A' + 10;
            }

            return -1;
        }


        public static JsonData ToBoolean(string json, ref int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index + 3 < json.Length && json.Substring(index, 4).Equals("true"))
            {
                index += 4;
                return new JsonData(JsonData.Type.Boolean) { json = "true" };
            }
            else if (index + 4 < json.Length && json.Substring(index, 5).Equals("false"))
            {
                index += 5;
                return new JsonData(JsonData.Type.Boolean) { json = "false" };
            }

            return null;
        }

        public static JsonData ToNumber(string json, ref int index)
        {
            return ToNumber(json, ref index, false);
        }

        /// <summary>
        /// 解析数字。json5 为 true 时额外允许省略整数部分（.5）或小数部分（5.）的写法。
        /// 无论哪种写法，最终保存的都是规范化后的合法 JSON 数字（如 0.5、5）。
        /// </summary>
        public static JsonData ToNumber(string json, ref int index, bool json5)
        {
            if (index < 0 || index >= json.Length)
            {
                return null;
            }

            int start = index;

            if (json[index] == '-')
            {
                index++;
            }
            else if (json[index] == '+')
            {
                start = index + 1;
                index++;
            }

            if (index + 2 < json.Length && json.Substring(index, 3).Equals("NaN"))
            {
                index += 3;
                return new JsonData(JsonData.Type.Number)
                {
                    json = json[start..index]
                };
            }
            else if (index + 7 < json.Length && json.Substring(index, 8).Equals("Infinity"))
            {
                index += 8;
                return new JsonData(JsonData.Type.Number)
                {
                    json = json[start..index]
                };
            }

            bool hasNumber = false;
            while (index < json.Length && char.IsNumber(json[index]))
            {
                index++;
                hasNumber = true;
            }

            bool leadingPoint = false;
            if (!hasNumber)
            {
                // JSON5 允许省略整数部分：.5
                if (json5 && index < json.Length && json[index] == '.')
                {
                    leadingPoint = true;
                }
                else
                {
                    return null;
                }
            }

            if (index < json.Length && json[index] == '.')
            {
                index++;
                int fractionStart = index;
                while (index < json.Length && char.IsDigit(json[index]))
                {
                    index++;
                }

                bool hasFraction = index > fractionStart;

                // 纯 JSON 必须写出小数位；JSON5 允许 "5."，但不允许只有小数点（如 ".e3"）
                if ((!hasFraction && !json5) || (!hasFraction && leadingPoint))
                {
                    return null;
                }
            }

            if (index < json.Length && (json[index] == 'e' || json[index] == 'E'))
            {
                index++;
                if (index < json.Length && (json[index] == '+' || json[index] == '-'))
                {
                    index++;
                }

                int exponentStart = index;
                while (index < json.Length && char.IsDigit(json[index]))
                {
                    index++;
                }

                if (index == exponentStart)
                {
                    return null;
                }
            }

            return new JsonData(JsonData.Type.Number)
            {
                json = NormalizeNumber(json[start..index])
            };
        }

        /// <summary>
        /// 把解析到的数字字面量规范化成合法 JSON 数字：
        /// 去掉多余的前导零（007 → 7），把 .5 补成 0.5，把 5. / 5.e3 写成 5 / 5e3。
        /// </summary>
        internal static string NormalizeNumber(string literal)
        {
            if (string.IsNullOrEmpty(literal))
            {
                return literal;
            }

            string sign = string.Empty;
            string body = literal;
            if (body[0] == '-' || body[0] == '+')
            {
                sign = body[0] == '-' ? "-" : string.Empty;
                body = body.Substring(1);
            }

            int firstMeaningful = 0;
            while (firstMeaningful + 1 < body.Length &&
                   body[firstMeaningful] == '0' &&
                   char.IsDigit(body[firstMeaningful + 1]))
            {
                firstMeaningful++;
            }

            body = body.Substring(firstMeaningful);

            if (body.StartsWith("."))
            {
                body = "0" + body;
            }

            body = body.Replace(".e", "e").Replace(".E", "E");
            if (body.EndsWith("."))
            {
                body = body.Substring(0, body.Length - 1);
            }

            return sign + body;
        }

        public static JsonData ToObject(string json, ref int index)
        {
            if (index < 0 || index >= json.Length || json[index] != '{')
            {
                return null;
            }

            int start = index++;
            JsonData obj = new JsonData(JsonData.Type.Object);
            bool hasItem = false;
            while (true)
            {
                SkipWhiteSpace(json, ref index);
                if (index >= json.Length)
                {
                    return null;
                }

                if (json[index] == '}')
                {
                    obj.json = json[start..(++index)];
                    return obj;
                }

                if (json[index] == ',')
                {
                    if (!hasItem)
                    {
                        return null;
                    }

                    index++;
                    SkipWhiteSpace(json, ref index);
                    if (index >= json.Length)
                    {
                        return null;
                    }

                    if (json[index] == '}')
                    {
                        obj.json = json[start..(++index)];
                        return obj;
                    }

                    continue;
                }

                if (json[index] != '"')
                {
                    return null;
                }

                // 复用字符串解析：键名同样需要处理 \" \\ \uXXXX 等转义
                JsonData keyNode = ToString_Double(json, ref index);
                if (keyNode == null)
                {
                    return null;
                }

                string key = JsonString.Unquote(keyNode.json);
                if (obj.HasKey(key))
                {
                    return null;
                }

                SkipWhiteSpace(json, ref index);
                if (index >= json.Length || json[index] != ':')
                {
                    return null;
                }

                index++;

                SkipWhiteSpace(json, ref index);
                if (index >= json.Length || json[index] == ',' || json[index] == '}')
                {
                    return null;
                }

                JsonData sub = Deserializer.ProcessJson(json, ref index);
                if (sub == null)
                {
                    return null;
                }

                obj[key] = sub;
                hasItem = true;

                SkipWhiteSpace(json, ref index);
                if (index >= json.Length)
                {
                    return null;
                }

                if (json[index] == '}')
                {
                    obj.json = json[start..(++index)];
                    return obj;
                }

                if (json[index] != ',')
                {
                    return null;
                }
            }
        }

        public static JsonData ToArray(string json, ref int index)
        {
            if (index < 0 || index >= json.Length || json[index] != '[')
            {
                return null;
            }

            int start = index++;
            JsonData arr = new JsonData(JsonData.Type.Array);
            bool hasItem = false;
            while (true)
            {
                SkipWhiteSpace(json, ref index);
                if (index >= json.Length)
                {
                    return null;
                }

                if (json[index] == ']')
                {
                    index++;
                    arr.json = json[start..index];
                    return arr;
                }

                if (json[index] == ',')
                {
                    if (!hasItem)
                    {
                        return null;
                    }

                    index++;
                    SkipWhiteSpace(json, ref index);
                    if (index >= json.Length)
                    {
                        return null;
                    }

                    if (json[index] == ']')
                    {
                        index++;
                        arr.json = json[start..index];
                        return arr;
                    }

                    continue;
                }

                JsonData sub = Deserializer.ProcessJson(json, ref index);
                if (sub == null)
                {
                    return null;
                }

                arr.Add(sub);
                hasItem = true;
                SkipWhiteSpace(json, ref index);
                if (index >= json.Length)
                {
                    return null;
                }

                if (json[index] == ']')
                {
                    index++;
                    arr.json = json[start..index];
                    return arr;
                }

                if (json[index] != ',')
                {
                    return null;
                }
            }
        }

        public static JsonData ToNone(string json, ref int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index + 3 < json.Length && json[index] == 'n' && json[index + 1] == 'u' && json[index + 2] == 'l' &&
                json[index + 3] == 'l')
            {
                index += 4;
                return new JsonData(JsonData.Type.None);
            }

            return null;
        }

        public static JsonData Unrecognized(string json, ref int index)
        {
            index += GetDistanceToNextComma(json, ref index);
            return new JsonData(JsonData.Type.None);
        }

        public static int GetDistanceToNextComma(string json, ref int startIndex)
        {
            for (int i = startIndex; i < json.Length; i++)
            {
                if (json[i] == ',' || json[i] == '}' || json[i] == ']')
                {
                    return i - startIndex;
                }
            }

            // 如果没有找到逗号，则返回剩余字符串的长度
            return json.Length - startIndex - 1;
        }
    }
}