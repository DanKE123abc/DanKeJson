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
    public class Json5Options
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
        
        public bool AddTailingCommaForObject { get; set; } = false;
        
        public bool AddTailingCommaForArray { get; set; } = false;
        
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
        public static JsonData ToData(string text, bool useComments = true, bool skipFileCheck = false)
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
        public static T ToData<T>(string text, bool useComments = true, bool skipFileCheck = false) where T : class, new()
        {
            return JSON.ToData<T>(text, useComments, skipFileCheck);
        }

        /// <summary>
        /// Deserializing JsonData to Json(String)
        /// About Json5 : https://json5.org
        /// </summary>
        /// <param name="json">the JsonData</param>
        /// <param name="options">JSON 5 format preferences</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(JsonData json, Json5Options options = null)
        {
            if (json == null)
            {
                return null;
            }
            
            if (options == null)
            {
                options = new Json5Options();
            }
            
            StringBuilder stringBuilder = new StringBuilder();
            ProcessData(json, stringBuilder, options);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deserializing Object to Json(String)
        /// About Json5 : https://json5.org
        /// </summary>
        /// <param name="jsonObject">object instantiated by the class</param>
        /// <param name="options">JSON 5 format preferences</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(object jsonObject, Json5Options options = null)
        {
            if (jsonObject == null)
            {
                return null;
            }
            
            if (options == null)
            {
                options = new Json5Options();
            }

            JsonData json = FromObject(jsonObject);
            StringBuilder stringBuilder = new StringBuilder();
            ProcessData(json, stringBuilder, options);
            return stringBuilder.ToString();
        }
        
        private static void ProcessData(JsonData json, StringBuilder builder, Json5Options options)
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
                    switch (options.StringQuoteStyle)
                    {
                        case Json5Options.StringQuoteType.DoubleQuote:
                            builder.Append("\"" + json.json[1..^1] + "\"");
                            break;
                        case Json5Options.StringQuoteType.SingleQuote:
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
                        switch (options.KeyNameStyle)
                        {
                            case Json5Options.KeyNameType.WithQuotes:
                                builder.Append("\"" + key + "\":");
                                break;
                            case Json5Options.KeyNameType.WithoutQuotes:
                                builder.Append(key + ":");
                                break;
                        }
                        ProcessData(json[key], builder, options);
                        builder.Append(',');
                    }

                    builder.Remove(builder.Length - 1, 1);
                    if (options.AddTailingCommaForObject)
                    {
                        builder.Append(',');
                    }
                    builder.Append('}');
                    break;
                case JsonData.Type.Array:
                    builder.Append('[');
                    foreach (var item in json.array)
                    {
                        ProcessData(item, builder, options);
                        builder.Append(',');
                    }
                    builder.Remove(builder.Length - 1, 1);
                    if (options.AddTailingCommaForArray)
                    {
                        builder.Append(',');
                    }
                    builder.Append(']');
                    break;

            }
        }
        
        
        // 这个函数和JSON类里的是一模一样的
        private static JsonData FromObject(object jsonObject)
        {
            JsonData json = new JsonData(JsonData.Type.Object);
            if (jsonObject == null)
            {
                json = new JsonData(JsonData.Type.None);
                return json;
            }
            System.Type type = jsonObject.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listType = type.GetGenericArguments()[0];
                var list = (IList)jsonObject;
                json = new JsonData(JsonData.Type.Array);

                foreach (var item in list)
                {
                    JsonData jsonDataItem;
                    if (item == null)
                    {
                        jsonDataItem = new JsonData(JsonData.Type.None);
                    }
                    else if (listType == typeof(string))
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
                            { json = item.ToString()! };
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
                        System.Type propertyType = propertyInfo.PropertyType;
                        if (propertyValue == null)
                        {
                            json[propertyInfo.Name] = new JsonData(JsonData.Type.None);
                            continue;
                        }
                        switch (Type.GetTypeCode(propertyType))
                        {
                            case TypeCode.String:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.String) { json = "\"" + propertyValue.ToString() + "\"" };
                                break;
                            case TypeCode.Boolean:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Boolean) { json = propertyValue.ToString().ToLower() };
                                break;
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                            case TypeCode.UInt16:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Number) { json = propertyValue.ToString()! };
                                break;
                            default:
                                json[propertyInfo.Name] = FromObject((object)propertyValue);
                                break;
                        }
                    }
                }
            }

            return json;
        }

        
    }


}