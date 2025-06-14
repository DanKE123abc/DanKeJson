using System.Globalization;
using System.Text;

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
                        json = sb.ToString()
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
                        default: // 未知转义序列保持原样
                            sb.Append('\\');
                            sb.Append(json[index - 1]);
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

            if (!hasNumber)
            {
                return null;
            }

            if (index >= json.Length || (json[index] != '.' &&
                                         json[index] != 'e' &&
                                         json[index] != 'E'))
            {
                return new JsonData(JsonData.Type.Number)
                {
                    json = json[start..index]
                };
            }

            if (json[index] == '.')
            {
                int pointIndex = index++;
                while (index < json.Length && char.IsDigit(json[index]))
                {
                    index++;
                }

                if (index == pointIndex + 1)
                {
                    return null;
                }

                if (index >= json.Length || (json[index] != 'e' && json[index] != 'E'))
                {
                    return new JsonData(JsonData.Type.Number)
                    {
                        json = json[start..index]
                    };
                }

            }

            int eIndex = index++;
            if (index < json.Length && (json[index] == '+' || json[index] == '-'))
            {
                index++;
            }

            while (index < json.Length && char.IsDigit(json[index]))
            {
                index++;
            }

            if (index == eIndex + 1)
            {
                return null;
            }

            return new JsonData(JsonData.Type.Number)
            {
                json = json[start..index]
            };

        }

        public static JsonData ToObject(string json, ref int index)
        {
            if (index < 0 || index >= json.Length || json[index] != '{')
            {
                return null;
            }

            int start = index++;
            JsonData obj = new JsonData(JsonData.Type.Object);
            do
            {
                if (json[index] == ',')
                {
                    index++;
                }

                SkipWhiteSpace(json, ref index);
                if (json[index] != '"')
                {
                    return null;
                }

                int keyIndex = index++;
                while (index < json.Length && json[index] != '"')
                {
                    index++;
                }

                if (index >= json.Length)
                {
                    return null;
                }

                string key = json[(keyIndex + 1)..(index++)];
                if (obj.HasKey(key))
                {
                    return null;
                }

                SkipWhiteSpace(json, ref index);
                if (json[index] != ':')
                {
                    return null;
                }

                index++;

                SkipWhiteSpace(json, ref index);
                JsonData sub = Serializer.ProcessJson(json, ref index);
                if (sub == null)
                {
                    return null;
                }

                obj[key] = sub;

                SkipWhiteSpace(json, ref index);

            } while (json[index] == ',');

            if (json[index] == '}')
            {
                obj.json = json[start..(++index)];
                return obj;
            }

            return null;
        }

        public static JsonData ToArray(string json, ref int index)
        {
            if (index < 0 || index >= json.Length || json[index] != '[')
            {
                return null;
            }

            int start = index++;
            JsonData arr = new JsonData(JsonData.Type.Array);
            do
            {
                if (json[index] == ',')
                {
                    index++;
                }

                SkipWhiteSpace(json, ref index);
                JsonData sub = Serializer.ProcessJson(json, ref index);
                if (sub == null)
                {
                    return null;
                }

                arr.Add(sub);
                SkipWhiteSpace(json, ref index);
            } while (index < json.Length && json[index] == ',');

            if (index >= json.Length || json[index] != ']')
            {
                return null;
            }

            index++;
            arr.json = json[start..index];
            return arr;
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