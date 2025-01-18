using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson.Json
{
    public class Deserializer
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
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.String)
                                    { json = "\"" + propertyValue.ToString() + "\"" };
                                break;
                            case TypeCode.Boolean:
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Boolean)
                                    { json = propertyValue.ToString().ToLower() };
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
                                json[propertyInfo.Name] = new JsonData(JsonData.Type.Number)
                                    { json = propertyValue.ToString()! };
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
                    builder.Append(json.json);
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
        
    }
}