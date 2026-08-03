using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using DanKeJson.Utils;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson.Json
{
    /// <summary>
    /// Serializer : serialize JsonData / .NET objects into Json text.
    /// </summary>
    public class Serializer
    {
        public static JsonData FromObject(object jsonObject)
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
                        string number = item is float f
                            ? f.ToString(CultureInfo.InvariantCulture)
                            : item is double d
                                ? d.ToString(CultureInfo.InvariantCulture)
                                : item.ToString();
                        jsonDataItem = new JsonData(JsonData.Type.Number) { json = number! };
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
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.String)
                                    { json = "\"" + propertyValue.ToString() + "\"" };
                                break;
                            case TypeCode.Boolean:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Boolean)
                                    { json = propertyValue.ToString().ToLower() };
                                break;
                            case TypeCode.Int32:
                            case TypeCode.Int64:
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.UInt32:
                            case TypeCode.UInt64:
                            case TypeCode.UInt16:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Number)
                                    { json = propertyValue.ToString() };
                                break;
                            case TypeCode.Single:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Number)
                                    { json = ((float)propertyValue).ToString(CultureInfo.InvariantCulture) };
                                break;
                            case TypeCode.Double:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Number)
                                    { json = ((double)propertyValue).ToString(CultureInfo.InvariantCulture) };
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

        public static void ProcessData(JsonData json, StringBuilder builder)
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
                    builder.Append('"');
                    builder.Append(JsonString.Escape(JsonString.Unquote(json.json)));
                    builder.Append('"');
                    break;
                case JsonData.Type.Boolean:
                    builder.Append(json.json);
                    break;
                case JsonData.Type.None:
                    builder.Append("null");
                    break;
                case JsonData.Type.Object:
                    builder.Append('{');
                    bool firstObject = true;
                    foreach (var key in json.map.Keys)
                    {
                        if (!firstObject)
                        {
                            builder.Append(',');
                        }

                        firstObject = false;
                        builder.Append('"');
                        builder.Append(JsonString.Escape(key));
                        builder.Append("\":");
                        ProcessData(json[key], builder);
                    }

                    builder.Append('}');
                    break;
                case JsonData.Type.Array:
                    builder.Append('[');
                    bool firstArray = true;
                    foreach (var item in json.array)
                    {
                        if (!firstArray)
                        {
                            builder.Append(',');
                        }

                        firstArray = false;
                        ProcessData(item, builder);
                    }

                    builder.Append(']');
                    break;

            }
        }
        
    }
}