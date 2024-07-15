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
    public class Json5Config
    {
        public enum KeyNameType
        {
            WithQuotes,
            WithoutQuotes
        }

        public enum StringQuoteType
        {
            SingleQuote,
            DoubleQuote
        }

        public bool AddCommaForObject { get; set; } = false;
        public bool AddCommaForArray { get; set; } = false;
        public KeyNameType KeyNameStyle { get; set; } = KeyNameType.WithQuotes;
        public StringQuoteType StringQuoteStyle { get; set; } = StringQuoteType.DoubleQuote;
        
        
    }

    /// <summary>
    /// DanKeJson : Serialization and Deserialization
    /// </summary>
    public static class JSON5
    {

        /// <summary>
        /// Serializing Json5(String) to JsonData
        /// About Json5 : https://json5.org
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="useComments">Using CommentParser.cs to Use Comments</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text, bool useComments = false, bool skipFileCheck = false)
        {
            return JSON.ToData(text, useComments, skipFileCheck);
        }

        /// <summary>
        /// Serializing Json5(String) to Class
        /// About Json5 : https://json5.org
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="useComments">Using CommentParser.cs to Use Comments</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        public static T ToData<T>(string text, bool useComments = false, bool skipFileCheck = false) where T : class, new()
        {
            return JSON.ToData<T>(text, useComments, skipFileCheck);
        }

        /// <summary>
        /// Deserializing JsonData to Json(String)
        /// About Json5 : https://json5.org
        /// </summary>
        /// <param name="json">the JsonData</param>
        /// <param name="config">JSON 5 format preferences</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(JsonData json, Json5Config config = null)
        {
            if (json == null)
            {
                return null;
            }
            
            if (config == null)
            {
                config = new Json5Config();
            }

            StringBuilder stringBuilder = new StringBuilder();
            ProcessData(json, stringBuilder, config);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deserializing Object to Json(String)
        /// About Json5 : https://json5.org
        /// </summary>
        /// <param name="jsonObject">object instantiated by the class</param>
        /// <param name="config">JSON 5 format preferences</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(object jsonObject, Json5Config config = null)
        {
            if (jsonObject == null)
            {
                return null;
            }
            
            if (config == null)
            {
                config = new Json5Config();
            }

            JsonData json = FromObject(jsonObject);
            StringBuilder stringBuilder = new StringBuilder();
            ProcessData(json, stringBuilder, config);
            return stringBuilder.ToString();
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

        private static void ProcessData(JsonData json, StringBuilder builder, Json5Config config)
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
                    switch (config.StringQuoteStyle)
                    {
                        case Json5Config.StringQuoteType.DoubleQuote:
                            builder.Append("\"" + json.json[1..^1] + "\"");
                            break;
                        case Json5Config.StringQuoteType.SingleQuote:
                            builder.Append("'" + json.json[1..^1] + "'");
                            break;
                    }
                    break;
                case JsonData.Type.Boolean:
                    builder.Append(json.json);
                    break;
                case JsonData.Type.None:
                    builder.Append("null");
                    break;
                case JsonData.Type.Object:
                    builder.Append('{');
                    foreach (var key in json.map.Keys)
                    {
                        switch (config.KeyNameStyle)
                        {
                            case Json5Config.KeyNameType.WithQuotes:
                                builder.Append("\"" + key + "\":");
                                break;
                            case Json5Config.KeyNameType.WithoutQuotes:
                                builder.Append(key + ":");
                                break;
                        }
                        ProcessData(json[key], builder, config);
                        builder.Append(',');
                    }

                    builder.Remove(builder.Length - 1, 1);
                    if (config.AddCommaForObject)
                    {
                        builder.Append(',');
                    }
                    builder.Append('}');
                    break;
                case JsonData.Type.Array:
                    builder.Append('[');
                    foreach (var item in json.array)
                    {
                        ProcessData(item, builder, config);
                        builder.Append(',');
                    }
                    builder.Remove(builder.Length - 1, 1);
                    if (config.AddCommaForArray)
                    {
                        builder.Append(',');
                    }
                    builder.Append(']');
                    break;

            }
        }
        
    }


}