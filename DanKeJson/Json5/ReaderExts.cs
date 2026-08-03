using System.Globalization;
using System.Text;
using DanKeJson.Json;
using static DanKeJson.Json.Reader;

#pragma warning disable CS8603

namespace DanKeJson.Json5
{
    public class ReaderExts
    {
        
        //json5单引号字符串
        public static JsonData ToString_Single(string json, ref int index)
        {
            if (index < 0 || index >= json.Length || json[index] != '\'')
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            int start = index; // 记录起始位置
            index++; // 跳过起始引号
            
            while (index < json.Length)
            {
                char current = json[index];
                
                if (current == '\'')
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
                        case '\'':
                            sb.Append('\'');
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

        //json5键名
        public static string ReadKey(string json, ref int index)
        {
            if (index < 0 || index >= json.Length)
            {
                return null;
            }

            if (json[index] == '"' || json[index] == '\'') //键名有引号
            {
                char quote = json[index];
                index++;
                int start = index;
                while (index < json.Length && json[index] != quote)
                {
                    index++;
                }

                if (index >= json.Length)
                {
                    return null;
                }

                return json.Substring(start, index++ - start);
            }
            else //键名无引号
            {
                int start = index;
                while (index < json.Length && (char.IsLetterOrDigit(json[index]) || json[index] == '_'))
                {
                    index++;
                }

                if (start == index)
                {
                    return null; // Empty key
                }

                return json.Substring(start, index - start);
            }
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

                string key = ReadKey(json, ref index);
                if (key == null)
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
                if (index >= json.Length || json[index] == ',' || json[index] == '}' || json[index] == ']')
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
    }
}
