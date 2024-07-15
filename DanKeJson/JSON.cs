#region Header

/*
* MIT License

Copyright (c) 2023 - present DanKe

* DanKeJson
a simple Json library for the .Net
JSON : https://json.org
 
* About Author
Name : DanKe
Address : Guangzhou City , Guangdong Province , China
Mail : danke1024@foxmail.com
Github : https://github.com/DanKE123abc
  */

#endregion

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;

namespace DanKeJson
{

    /// <summary>
    /// DanKeJson : Serialization and Deserialization
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// Serializing Json(String) to JsonData
        /// About Json : https://json.org
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="useComments">Using CommentParser.cs to Use Comments</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text, bool useComments = false, bool skipFileCheck = false)
        {

            if (!skipFileCheck)
            {
                if (IsFilePath(text))
                {
                    text = File.ReadAllText(text);
                }
            }
            if (useComments)
            {
                text = CommentParser.RemoveComments(text);
            }
            
            int index = 0;
            JsonData json = ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return json;
            }
            
            return null;
        }

        /// <summary>
        /// Serializing Json(String) to Class
        /// About Json : https://json.org
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="useComments">Using CommentParser.cs to Use Comments</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        public static T ToData<T>(string text, bool useComments = false, bool skipFileCheck = false) where T : class, new()
        {
            if (!skipFileCheck)
            {
                if (IsFilePath(text))
                {
                    text = File.ReadAllText(text);
                }
            }
            if (useComments)
            {
                text = CommentParser.RemoveComments(text);
            }

            int index = 0;
            JsonData json = ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return (T)FromJson(json, typeof(T));
            }

            return default(T);
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
        /// Deserializing Object to Json(String)
        /// About Json : https://json.org
        /// </summary>
        /// <param name="jsonObject">object instantiated by the class</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(object jsonObject)
        {
            if (jsonObject == null)
            {
                return null;
            }

            JsonData json = FromObject(jsonObject);
            StringBuilder stringBuilder = new StringBuilder();
            ProcessData(json, stringBuilder);
            return stringBuilder.ToString();
        }

        private static bool IsFilePath(string path)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                return false;
            }
            else
            {
                if (!File.Exists(path))
                {
                    return false;
                }
            }
            return true;
        }

        private static object FromJson(JsonData json, Type type)
        {
            object dataclass = Activator.CreateInstance(type);
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listType = type.GetGenericArguments()[0];
                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));

                switch (Type.GetTypeCode(listType))
                {
                    case TypeCode.String:
                        foreach (var item in json.array)
                        {
                            string stringValue = item.json;
                            list.Add(stringValue);
                        }
                        break;
                    case TypeCode.Boolean:
                        foreach (var item in json.array)
                        {
                            bool.TryParse(item.json, out bool boolValue);
                            list.Add(boolValue);
                        }
                        break;
                                        case TypeCode.Int32:
                        foreach (var item in json.array)
                        {
                            int.TryParse(item.json, out int intValue);
                            list.Add(intValue);
                        }
                        break;
                    case TypeCode.Int64:
                        foreach (var item in json.array)
                        {
                            long.TryParse(item.json, out long longValue);
                            list.Add(longValue);
                        }
                        break;
                    case TypeCode.Single:
                        foreach (var item in json.array)
                        {
                            float.TryParse(item.json, out float floatValue);
                            list.Add(floatValue);
                        }
                        break;
                    case TypeCode.Double:
                        foreach (var item in json.array)
                        {
                            double.TryParse(item.json, out double doubleValue);
                            list.Add(doubleValue);
                        }
                        break;
                    case TypeCode.SByte:
                        foreach (var item in json.array)
                        {
                            sbyte.TryParse(item.json, out sbyte sbyteValue);
                            list.Add(sbyteValue);
                        }
                        break;
                    case TypeCode.Int16:
                        foreach (var item in json.array)
                        {
                            short.TryParse(item.json, out short shortValue);
                            list.Add(shortValue);
                        }
                        break;
                    case TypeCode.UInt32:
                        foreach (var item in json.array)
                        {
                            uint.TryParse(item.json, out uint uintValue);
                            list.Add(uintValue);
                        }
                        break;
                    case TypeCode.UInt64:
                        foreach (var item in json.array)
                        {
                            ulong.TryParse(item.json, out ulong ulongValue);
                            list.Add(ulongValue);
                        }
                        break;
                    case TypeCode.UInt16:
                        foreach (var item in json.array)
                        {
                            ushort.TryParse(item.json, out ushort ushortValue);
                            list.Add(ushortValue);
                        }
                        break;
                    default:
                        foreach (var item in json.array)
                        {
                            switch (item.type)
                            {
                                case JsonData.Type.Array:
                                case JsonData.Type.Object:
                                    list.Add(FromJson(item, listType));
                                    break;
                                case JsonData.Type.None:
                                    list.Add(null);
                                    break;
                            }
                        }
                        break;
                }
                return list;
            }
            else
            {
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.CanWrite)
                    {
                        Type propertyType = propertyInfo.PropertyType;
                        switch (Type.GetTypeCode(propertyType))
                        {
                            case TypeCode.String:
                                string stringValue = json[propertyInfo.Name];
                                propertyInfo.SetValue(dataclass, stringValue);
                                break;
                            case TypeCode.Boolean:
                                bool.TryParse(json[propertyInfo.Name].json, out bool boolValue);
                                propertyInfo.SetValue(dataclass, boolValue);
                                break;
                            case TypeCode.Int32:
                                int.TryParse(json[propertyInfo.Name].json, out int intValue);
                                propertyInfo.SetValue(dataclass, intValue);
                                break;
                            case TypeCode.Int64:
                                long.TryParse(json[propertyInfo.Name].json, out long longValue);
                                propertyInfo.SetValue(dataclass, longValue);
                                break;
                            case TypeCode.Single:
                                float.TryParse(json[propertyInfo.Name].json, out float floatValue);
                                propertyInfo.SetValue(dataclass, floatValue);
                                break;
                            case TypeCode.Double:
                                double.TryParse(json[propertyInfo.Name].json, out double doubleValue);
                                propertyInfo.SetValue(dataclass, doubleValue);
                                break;
                            case TypeCode.SByte:
                                sbyte.TryParse(json[propertyInfo.Name].json, out sbyte sbyteValue);
                                propertyInfo.SetValue(dataclass, sbyteValue);
                                break;
                            case TypeCode.Int16:
                                short.TryParse(json[propertyInfo.Name].json, out short shortValue);
                                propertyInfo.SetValue(dataclass, shortValue);
                                break;
                            case TypeCode.UInt32:
                                uint.TryParse(json[propertyInfo.Name].json, out uint uintValue);
                                propertyInfo.SetValue(dataclass, uintValue);
                                break;
                            case TypeCode.UInt64:
                                ulong.TryParse(json[propertyInfo.Name].json, out ulong ulongValue);
                                propertyInfo.SetValue(dataclass, ulongValue);
                                break;
                            case TypeCode.UInt16:
                                ushort.TryParse(json[propertyInfo.Name].json, out ushort ushortValue);
                                propertyInfo.SetValue(dataclass, ushortValue);
                                break;
                            default:
                                if (propertyType.IsGenericType &&
                                    propertyType.GetGenericTypeDefinition() == typeof(List<>))
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
                                    else if (listType.IsClass)
                                    {
                                        IList list =
                                            (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
                                        foreach (var item in json[propertyInfo.Name].array)
                                        {
                                            var obj = FromJson(item, listType);
                                            list.Add(obj);
                                        }

                                        propertyInfo.SetValue(dataclass, list);
                                    }
                                    else if (listType.IsGenericType &&
                                             listType.GetGenericTypeDefinition() == typeof(List<>))
                                    {
                                        IList list =
                                            (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listType));
                                        foreach (var item in json[propertyInfo.Name].array)
                                        {
                                            var obj = FromJson(item, listType);
                                            list.Add(obj);
                                        }

                                        IList convertedList = list.Cast<object>().ToList();
                                        propertyInfo.SetValue(dataclass, convertedList);
                                    }

                                }
                                else if (propertyType.IsClass)
                                {
                                    var propertyValue = FromJson(json[propertyInfo.Name].ToString(), propertyType);
                                    propertyInfo.SetValue(dataclass, propertyValue);
                                }

                                break;
                        }
                    }
                }

                return dataclass;

            }
        }

        private static JsonData FromObject(object jsonObject)
        {
            JsonData json = new JsonData(JsonData.Type.Object);
            System.Type type = jsonObject.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listType = type.GetGenericArguments()[0];
                var list = (IList)jsonObject;
                json = new JsonData(JsonData.Type.Array);

                foreach (var item in list)
                {
                    JsonData jsonDataItem;

                    if (listType == typeof(string))
                    {
                        jsonDataItem = new JsonData(JsonData.Type.String)
                            { json = "\"" + item.ToString() + "\"" };
                    }
                    else if (listType == typeof(int) || listType == typeof(long) ||
                             listType == typeof(float) || listType == typeof(double) ||
                             listType == typeof(sbyte) || listType == typeof(short) ||
                             listType == typeof(uint) || listType == typeof(ulong) ||
                             listType == typeof(ushort))
                    {
                        jsonDataItem = new JsonData(JsonData.Type.Number)
                            { json = item.ToString() };
                    }
                    else if (listType == typeof(bool))
                    {
                        jsonDataItem = new JsonData(JsonData.Type.Boolean)
                            { json = item.ToString().ToLower() };
                    }
                    else
                    {
                        jsonDataItem = FromObject(item);
                    }

                    json.array.Add(jsonDataItem);
                }
            }
            else
            {
                foreach (PropertyInfo propertyInfo in type.GetProperties())
                {
                    if (propertyInfo.CanRead)
                    {
                        object propertyValue = propertyInfo.GetValue(jsonObject);
                        if (propertyValue == null) continue;

                        System.Type propertyType = propertyInfo.PropertyType;
                        switch (Type.GetTypeCode(propertyType))
                        {
                            case TypeCode.String:
                                json[propertyInfo.Name] = (string)propertyValue;
                                break;
                            case TypeCode.Boolean:
                                json[propertyInfo.Name] = (bool)propertyValue;
                                break;
                            case TypeCode.Int32:
                                json[propertyInfo.Name] = (int)propertyValue;
                                break;
                            case TypeCode.Int64:
                                json[propertyInfo.Name] = (long)propertyValue;
                                break;
                            case TypeCode.Single:
                                json[propertyInfo.Name] = (float)propertyValue;
                                break;
                            case TypeCode.Double:
                                json[propertyInfo.Name] = (double)propertyValue;
                                break;
                            case TypeCode.SByte:
                                json[propertyInfo.Name] = (sbyte)propertyValue;
                                break;
                            case TypeCode.Int16:
                                json[propertyInfo.Name] = (short)propertyValue;
                                break;
                            case TypeCode.UInt32:
                                json[propertyInfo.Name] = (uint)propertyValue;
                                break;
                            case TypeCode.UInt64:
                                json[propertyInfo.Name] = (ulong)propertyValue;
                                break;
                            case TypeCode.UInt16:
                                json[propertyInfo.Name] = (ushort)propertyValue;
                                break;
                            default:
                                // 确保属性是List<T>
                                if (propertyInfo.PropertyType.IsGenericType &&
                                    propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    var listType = propertyInfo.PropertyType.GenericTypeArguments[0];
                                    var list = (IList)propertyValue;
                                    json[propertyInfo.Name] = new JsonData(JsonData.Type.Array);

                                    foreach (var item in list)
                                    {
                                        JsonData jsonDataItem;

                                        if (listType == typeof(string))
                                        {
                                            jsonDataItem = new JsonData(JsonData.Type.String)
                                                { json = "\"" + item.ToString() + "\"" };
                                        }
                                        else if (listType == typeof(int) || listType == typeof(long) ||
                                                 listType == typeof(float) || listType == typeof(double) ||
                                                 listType == typeof(sbyte) || listType == typeof(short) ||
                                                 listType == typeof(uint) || listType == typeof(ulong) ||
                                                 listType == typeof(ushort))
                                        {
                                            jsonDataItem = new JsonData(JsonData.Type.Number) 
                                                { json = item.ToString() };
                                        }
                                        else if (listType == typeof(bool))
                                        {
                                            jsonDataItem = new JsonData(JsonData.Type.Boolean)
                                                { json = item.ToString().ToLower() };
                                        }
                                        else
                                        {
                                            jsonDataItem = FromObject(item);
                                        }

                                        json[propertyInfo.Name].array.Add(jsonDataItem);
                                    }
                                }
                                else if (propertyType.IsClass)
                                {
                                    json[propertyInfo.Name] = FromObject((object)propertyValue);
                                }

                                break;
                        }
                    }
                }
            }

            return json;
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
                case JsonData.Type.None :
                    builder.Append("null");
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
            else if (cur == '\'')
            {
                //String (Single)
                jsonData = ToString_Single(json, ref index);
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
            else if (cur == 'n')
            {
                //None
                jsonData = ToNone(json, ref index);
            }
            else
            {
                jsonData = Unrecognized(json, ref index);
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
        
        private static JsonData ToString_Single(string json, ref int index)
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
                JsonData sub = ProcessJson(json, ref index);
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
        
        private static string ReadKey(string json, ref int index)
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

        private static bool IsObjectEnd(string json, ref int index)
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
            } while (IsArrayEnd(json, ref index));

            if (index >= json.Length || json[index] != ']')
            {
                return null;
            }

            index++;
            arr.json = json[start..index];
            return arr;
        }
        
        private static bool IsArrayEnd(string json, ref int index)
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
        
        private static JsonData ToNone(string json, ref int index)
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

        private static JsonData Unrecognized(string json, ref int index)
        {
            index += GetDistanceToNextComma(json, ref index);
            return new JsonData(JsonData.Type.None);
        }

        private static int GetDistanceToNextComma(string json, ref int startIndex)
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
