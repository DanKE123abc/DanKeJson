/*
    DanKeJson
a simple Json library for the .Net

JSON : https://json.org

    MIT License

Copyright (c) 2023-forever DanKe

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. 
 
    About Author
Name : DanKe
Address : Guangzhou City , Guangdong Provice , China
Mail : danke1024@foxmail.com
Github : https://github.com/DanKE123abc
  */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;

namespace DanKeJson
{
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600

    /// <summary>
    /// DanKeJson : Serialization and Deialization
    /// </summary>
    public static class JSON
    {
        
        /// <summary>
        /// Serializing Json(String) to JsonData
        /// About Json : https://json.org
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text)
        {
            text = CommentParser.RemoveComments(text);
            int index = 0;
            JsonData json = ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return json;
            }

            return null;
        }
        
        /// <summary>
        /// Deserializing JsonData to Json(String)
        /// About Json : https://json.org
        /// </summary>
        /// <param name="json">the JsonData</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(JsonData json)
        {
            if (json == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            ProcessData(json, stringBuilder);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Beta!
        /// Serializing Json(String) to Class
        /// About Json : https://json.org
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        public static T ToData<T>(string text) where T : class, new()
        {
            text = CommentParser.RemoveComments(text);
            int index = 0;
            JsonData json = ProcessJson(text, ref index);
            if (index == text.Length)
            {
                T dataclass = new T();
                System.Type type = typeof(T);
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.CanWrite)
                    {
                        System.Type propertyType = propertyInfo.PropertyType;
                        if (propertyType == typeof(string))
                        {
                            propertyInfo.SetValue(dataclass, json[propertyInfo.Name].json[1..^1]);
                        }
                        else if (propertyType == typeof(bool))
                        {
                            bool.TryParse(json[propertyInfo.Name].json, out bool value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(int))
                        {
                            int.TryParse(json[propertyInfo.Name].json, out int value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(long))
                        {
                            long.TryParse(json[propertyInfo.Name].json, out long value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(float))
                        {
                            float.TryParse(json[propertyInfo.Name].json, out float value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(double))
                        {
                            double.TryParse(json[propertyInfo.Name].json, out double value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(sbyte))
                        {
                            sbyte.TryParse(json[propertyInfo.Name].json, out sbyte value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(short))
                        {
                            short.TryParse(json[propertyInfo.Name].json, out short value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(uint))
                        {
                            uint.TryParse(json[propertyInfo.Name].json, out uint value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(ulong))
                        {
                            ulong.TryParse(json[propertyInfo.Name].json, out ulong value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType == typeof(ushort))
                        {
                            ushort.TryParse(json[propertyInfo.Name].json, out ushort value);
                            propertyInfo.SetValue(dataclass, value);
                        }
                        else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            Type listType = propertyType.GetGenericArguments()[0];
                            if (listType == typeof(string))
                            {
                                List<string> list = new List<string>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(bool))
                            {
                                List<bool> list = new List<bool>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(int))
                            {
                                List<int> list = new List<int>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(long))
                            {
                                List<long> list = new List<long>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(float))
                            {
                                List<float> list = new List<float>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(double))
                            {
                                List<double> list = new List<double>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(sbyte))
                            {
                                List<sbyte> list = new List<sbyte>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(short))
                            {
                                List<short> list = new List<short>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(uint))
                            {
                                List<uint> list = new List<uint>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(ulong))
                            {
                                List<ulong> list = new List<ulong>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (listType == typeof(ushort))
                            {
                                List<ushort> list = new List<ushort>();
                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item);
                                }
                                propertyInfo.SetValue(dataclass, list);
                            }
                            else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                            {
                                List<object> list = new List<object>();

                                foreach (var item in json[propertyInfo.Name].array)
                                {
                                    list.Add(item.json);
                                }
                                IList convertedList = list.Cast<object>().ToList();
                                propertyInfo.SetValue(dataclass, convertedList);
                            }
                        }
                        else
                        {
                            //todo
                        }
                    }
                }

                return dataclass;
            }

            return default(T);
        }

        
        /// <summary>
        /// Beta!
        /// Deserializing Object to Json(String)
        /// About Json : https://json.org
        /// </summary>
        /// <param name="jsonObject">Csharp Object</param>
        /// <returns></returns>
        public static string ToJson(object jsonObject)
        {
            if (jsonObject == null)
            {
                return null;
            }

            JsonData json = new JsonData(JsonData.Type.Object);
            StringBuilder stringBuilder = new StringBuilder();
            System.Type type = jsonObject.GetType();
            foreach (PropertyInfo propertyInfo in type.GetProperties())
            {
                if (propertyInfo.CanWrite)
                {
                    System.Type propertyType = propertyInfo.PropertyType;
                    if (propertyType == typeof(string))
                    {
                        json[propertyInfo.Name] = (string)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(bool))
                    {
                        json[propertyInfo.Name] = (bool)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(int))
                    {
                        json[propertyInfo.Name] = (int)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(long))
                    {
                        json[propertyInfo.Name] = (long)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(float))
                    {
                        json[propertyInfo.Name] = (float)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(double))
                    {
                        json[propertyInfo.Name] = (double)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(sbyte))
                    {
                        json[propertyInfo.Name] = (sbyte)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(short))
                    {
                        json[propertyInfo.Name] = (short)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(uint))
                    {
                        json[propertyInfo.Name] = (uint)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(ulong))
                    {
                        json[propertyInfo.Name] = (ulong)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType == typeof(ushort))
                    {
                        json[propertyInfo.Name] = (ushort)propertyInfo.GetValue(jsonObject)!;
                    }
                    else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {

                        Type listType = propertyType.GetGenericArguments()[0];
                        if (listType == typeof(string))
                        {
                            List<string> propertyList = (List<string>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(bool))
                        {
                            List<bool> propertyList = (List<bool>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(int))
                        {
                            List<int> propertyList = (List<int>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(long))
                        {
                            List<long> propertyList = (List<long>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(float))
                        {
                            List<float> propertyList = (List<float>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(double))
                        {
                            List<double> propertyList = (List<double>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(sbyte))
                        {
                            List<sbyte> propertyList = (List<sbyte>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(short))
                        {
                            List<short> propertyList = (List<short>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(uint))
                        {
                            List<uint> propertyList = (List<uint>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(ulong))
                        {
                            List<ulong> propertyList = (List<ulong>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (listType == typeof(ushort))
                        {
                            List<ushort> propertyList = (List<ushort>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                            }
                        }
                        else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            List<object> propertyList = (List<object>)jsonObject.GetType().GetProperty(propertyInfo.Name).GetValue(jsonObject, null);
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);
                            foreach (var item in propertyList)
                            {
                                json[propertyInfo.Name].Add(item.ToString());
                                //服了，有bug
                            }
                        }
                    }
                    else
                    {
                        //todo
                    }
                    
                }
            }

            ProcessData(json, stringBuilder);
            return stringBuilder.ToString();
        }

        private static void ProcessData(JsonData json, StringBuilder builder)
        {
            if (json == null || builder == null)
            {
                return;
            }

            switch (json.type)
            {
                case JsonData.Type.Number:
                    builder.Append(json.json);
                    break;
                case JsonData.Type.String:
                    builder.Append(json.json);
                    break;
                case JsonData.Type.Boolean:
                    builder.Append(json.json);
                    break;
                case JsonData.Type.Object:
                    builder.Append('{');
                    foreach (var key in json.map.Keys)
                    {
                        builder.Append("\"" + key + "\":");
                        ProcessData(json[key], builder);
                        builder.Append(',');
                    }

                    builder.Remove(builder.Length - 1, 1);
                    builder.Append('}');
                    break;
                case JsonData.Type.Array:
                    builder.Append('[');
                    foreach (var item in json.array)
                    {
                        ProcessData(item, builder);
                        builder.Append(',');
                    }

                    builder.Remove(builder.Length - 1, 1);
                    builder.Append(']');
                    break;
            }
        }

        private static JsonData ProcessJson(string json, ref int index)
        {
            if (index < 0 || index >= json.Length)
            {
                return null;
            }

            SkipWhiteSpace(json, ref index);
            char cur = json[index];
            JsonData jsonData = null;
            if (cur == '\"')
            {
                //String
                jsonData = ToString(json, ref index);
            }
            else if (cur == 't' || cur == 'f')
            {
                //Boolean
                jsonData = ToBoolean(json, ref index);
            }
            else if (cur == '-' || char.IsDigit(cur))
            {
                //Number
                jsonData = ToNumber(json, ref index);
            }
            else if (cur == '{')
            {
                //Object
                jsonData = ToObject(json, ref index);
            }
            else if (cur == '[')
            {
                //Array
                jsonData = ToArray(json, ref index);
            }

            SkipWhiteSpace(json, ref index);

            return jsonData;
        }

        private static void SkipWhiteSpace(string json, ref int index)
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

        private static JsonData ToString(string json, ref int index)
        {
            if (index < 0 || index >= json.Length || json[index] != '\"')
            {
                return null;
            }

            int start = index++;

            while (index < json.Length && json[index] != '\"')
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

        private static JsonData ToBoolean(string json, ref int index)
        {
            if (index < 0)
            {
                return null;
            }

            if (index + 3 < json.Length && json[index] == 't' && json[index + 1] == 'r' && json[index + 2] == 'u' &&
                json[index + 3] == 'e')
            {
                index += 4;
                return new JsonData(JsonData.Type.Boolean) { json = "true" };
            }
            else if (index + 4 < json.Length && json[index] == 'f' && json[index + 1] == 'a' &&
                     json[index + 2] == 'l' && json[index + 3] == 's' && json[index + 4] == 'e')
            {
                index += 5;
                return new JsonData(JsonData.Type.Boolean) { json = "false" };
            }

            return null;
        }

        private static JsonData ToNumber(string json, ref int index)
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

        private static JsonData ToObject(string json, ref int index)
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
                JsonData sub = ProcessJson(json, ref index);
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

        private static JsonData ToArray(string json, ref int index)
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
                JsonData sub = ProcessJson(json, ref index);
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

    }
}
