using DanKeJson.Json;
using static DanKeJson.Json.Reader;

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

            int start = index++;

            while (index < json.Length && json[index] != '\'')
            {
                index++;
            }

            if (index >= json.Length)
            {
                return null;
            }

            return new JsonData(JsonData.Type.String)
            {
                json = json[start..(++index)]
            };

        }

        //json5键名
        public static string ReadKey(string json, ref int index)
        {
            if (json[index] == '"') //键名有引号
            {
                index++;
                int start = index;
                while (index < json.Length && json[index] != '"')
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
                while (index < json.Length && char.IsLetterOrDigit(json[index]) || json[index] == '_')
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
            do
            {
                SkipWhiteSpace(json, ref index);

                int keyIndex = index;
                string key = ReadKey(json, ref index);
                if (key == null)
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

            } while (IsObjectEnd(json, ref index));

            if (json[index] == '}')
            {
                obj.json = json[start..(++index)];
                return obj;
            }

            return null;
        }

        public static bool IsObjectEnd(string json, ref int index)
        {
            SkipWhiteSpace(json, ref index);

            if (json[index] == ',')
            {
                index++;
                SkipWhiteSpace(json, ref index);
                if (json[index] == '}')
                {
                    return false;
                }

                return true;
            }

            return false;
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
            } while (IsArrayEnd(json, ref index));

            if (index >= json.Length || json[index] != ']')
            {
                return null;
            }

            index++;
            arr.json = json[start..index];
            return arr;
        }

        public static bool IsArrayEnd(string json, ref int index)
        {
            SkipWhiteSpace(json, ref index);

            if (index < json.Length && json[index] == ',')
            {
                index++;
                SkipWhiteSpace(json, ref index);
                if (json[index] == ']')
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}