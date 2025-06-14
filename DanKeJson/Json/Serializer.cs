using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static DanKeJson.Json.Reader;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson
{
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonProperty : Attribute
    {
        public string Name { get; }

        public JsonProperty(string name)
        {
            Name = name;
        }
    }
}

namespace DanKeJson.Json
{
    public class Serializer
    {
        public static object FromJson(JsonData json, Type type)
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
                // 获取所有属性并按自定义特性排序
                var properties = type.GetProperties()
                    .Select(p => new
                    {
                        PropertyInfo = p,
                        JsonProperty = p.GetCustomAttribute<JsonProperty>()
                    })
                    .OrderByDescending(p => p.JsonProperty != null)
                    .ThenBy(p => p.PropertyInfo.Name)
                    .Select(p => p.PropertyInfo)
                    .ToList();

                foreach (PropertyInfo propertyInfo in properties)
                {
                    if (propertyInfo.CanWrite)
                    {
                        Type propertyType = propertyInfo.PropertyType;
                        string propertyName = propertyInfo.Name;

                        // 获取自定义特性的名称
                        var jsonProperty = propertyInfo.GetCustomAttribute<JsonProperty>();
                        if (jsonProperty != null)
                        {
                            propertyName = jsonProperty.Name;
                        }
                        
                        if ((Nullable.GetUnderlyingType(propertyType) ?? propertyType) == typeof(JsonData))
                        {
                            propertyInfo.SetValue(dataclass, json[propertyName]);
                        }
                        
                        switch (Type.GetTypeCode(propertyType))
                        {
                            case TypeCode.String:
                                string stringValue = json[propertyName];
                                propertyInfo.SetValue(dataclass, stringValue);
                                break;
                            case TypeCode.Boolean:
                                bool.TryParse(json[propertyName].json, out bool boolValue);
                                propertyInfo.SetValue(dataclass, boolValue);
                                break;
                            case TypeCode.Int32:
                                int.TryParse(json[propertyName].json, out int intValue);
                                propertyInfo.SetValue(dataclass, intValue);
                                break;
                            case TypeCode.Int64:
                                long.TryParse(json[propertyName].json, out long longValue);
                                propertyInfo.SetValue(dataclass, longValue);
                                break;
                            case TypeCode.Single:
                                float.TryParse(json[propertyName].json, out float floatValue);
                                propertyInfo.SetValue(dataclass, floatValue);
                                break;
                            case TypeCode.Double:
                                double.TryParse(json[propertyName].json, out double doubleValue);
                                propertyInfo.SetValue(dataclass, doubleValue);
                                break;
                            case TypeCode.SByte:
                                sbyte.TryParse(json[propertyName].json, out sbyte sbyteValue);
                                propertyInfo.SetValue(dataclass, sbyteValue);
                                break;
                            case TypeCode.Int16:
                                short.TryParse(json[propertyName].json, out short shortValue);
                                propertyInfo.SetValue(dataclass, shortValue);
                                break;
                            case TypeCode.UInt32:
                                uint.TryParse(json[propertyName].json, out uint uintValue);
                                propertyInfo.SetValue(dataclass, uintValue);
                                break;
                            case TypeCode.UInt64:
                                ulong.TryParse(json[propertyName].json, out ulong ulongValue);
                                propertyInfo.SetValue(dataclass, ulongValue);
                                break;
                            case TypeCode.UInt16:
                                ushort.TryParse(json[propertyName].json, out ushort ushortValue);
                                propertyInfo.SetValue(dataclass, ushortValue);
                                break;
                            default:
                                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>))
                                {
                                    JsonData nestedJson = json[propertyName];
                                    if (nestedJson.type == JsonData.Type.Object)
                                    {
                                        var nestedObject = FromJson(nestedJson, propertyType);
                                        propertyInfo.SetValue(dataclass, nestedObject);
                                    }
                                    else if (nestedJson.type == JsonData.Type.Array)
                                    {
                                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(propertyType.GetGenericArguments()[0]));
                                        foreach (var item in nestedJson.array)
                                        {
                                            var obj = FromJson(item, propertyType.GetGenericArguments()[0]);
                                            list.Add(obj);
                                        }
                                        propertyInfo.SetValue(dataclass, list);
                                    }
                                }
                                break;
                        }
                    }
                }

                return dataclass;
            }
        }

        public static JsonData ProcessJson(string json, ref int index)
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
                //String (Double/Standard)
                jsonData = ToString_Double(json, ref index);
            }
            else if (cur == 't' || cur == 'f')
            {
                //Boolean
                jsonData = ToBoolean(json, ref index);
            }
            else if (cur == '-' || cur == '+' || char.IsDigit(cur))
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
    }
}