using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DanKeJson.Utils;
using static DanKeJson.Json.Reader;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
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
    /// <summary>
    /// Deserializer : parse Json text into JsonData / .NET objects.
    /// </summary>
    public class Deserializer
    {
        public static object FromJson(JsonData json, Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                var listType = type.GetGenericArguments()[0];
                return ListFromArray(json, listType);
            }

            if (type.IsArray)
            {
                return ArrayFromJson(json, type.GetElementType());
            }

            if (json == null || json.type == JsonData.Type.None)
            {
                return null;
            }

            object dataclass = Activator.CreateInstance(type);

            // 获取所有字段和属性，并按自定义特性排序
            var members = GetMembers(type);

            foreach (MemberInfo member in members)
            {
                if (member is PropertyInfo propertyInfo && !propertyInfo.CanWrite)
                {
                    continue;
                }

                if (member is FieldInfo fieldInfo && fieldInfo.IsInitOnly)
                {
                    continue;
                }

                Type memberType = GetMemberType(member);
                string memberName = member.Name;

                // 获取自定义特性的名称
                var jsonProperty = member.GetCustomAttribute<JsonProperty>();
                if (jsonProperty != null)
                {
                    memberName = jsonProperty.Name;
                }

                JsonData propertyJson = json[memberName];
                if (propertyJson == null)
                {
                    continue;
                }

                if ((Nullable.GetUnderlyingType(memberType) ?? memberType) == typeof(JsonData))
                {
                    SetMemberValue(member, dataclass, propertyJson);
                    continue;
                }

                switch (Type.GetTypeCode(memberType))
                {
                    case TypeCode.String:
                        string stringValue = propertyJson;
                        SetMemberValue(member, dataclass, stringValue);
                        break;
                    case TypeCode.Boolean:
                        bool.TryParse(propertyJson.json, out bool boolValue);
                        SetMemberValue(member, dataclass, boolValue);
                        break;
                    case TypeCode.Int32:
                        int.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue);
                        SetMemberValue(member, dataclass, intValue);
                        break;
                    case TypeCode.Int64:
                        long.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longValue);
                        SetMemberValue(member, dataclass, longValue);
                        break;
                    case TypeCode.Single:
                        float.TryParse(propertyJson.json, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue);
                        SetMemberValue(member, dataclass, floatValue);
                        break;
                    case TypeCode.Double:
                        double.TryParse(propertyJson.json, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue);
                        SetMemberValue(member, dataclass, doubleValue);
                        break;
                    case TypeCode.SByte:
                        sbyte.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte sbyteValue);
                        SetMemberValue(member, dataclass, sbyteValue);
                        break;
                    case TypeCode.Int16:
                        short.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out short shortValue);
                        SetMemberValue(member, dataclass, shortValue);
                        break;
                    case TypeCode.UInt32:
                        uint.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint uintValue);
                        SetMemberValue(member, dataclass, uintValue);
                        break;
                    case TypeCode.UInt64:
                        ulong.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong ulongValue);
                        SetMemberValue(member, dataclass, ulongValue);
                        break;
                    case TypeCode.UInt16:
                        ushort.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort ushortValue);
                        SetMemberValue(member, dataclass, ushortValue);
                        break;
                    case TypeCode.Decimal:
                        decimal.TryParse(JsonString.Unquote(propertyJson.json), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue);
                        SetMemberValue(member, dataclass, decimalValue);
                        break;
                    case TypeCode.DateTime:
                        DateTime.TryParse(JsonString.Unquote(propertyJson.json), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dateTimeValue);
                        SetMemberValue(member, dataclass, dateTimeValue);
                        break;
                    default:
                        if (memberType == typeof(TimeSpan))
                        {
                            TimeSpan.TryParse(JsonString.Unquote(propertyJson.json), CultureInfo.InvariantCulture, out TimeSpan timeSpanValue);
                            SetMemberValue(member, dataclass, timeSpanValue);
                        }
                        else if (memberType == typeof(Guid))
                        {
                            Guid.TryParse(JsonString.Unquote(propertyJson.json), out Guid guidValue);
                            SetMemberValue(member, dataclass, guidValue);
                        }
                        else if (memberType == typeof(DateTimeOffset))
                        {
                            DateTimeOffset.TryParse(JsonString.Unquote(propertyJson.json), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset dateTimeOffsetValue);
                            SetMemberValue(member, dataclass, dateTimeOffsetValue);
                        }
                        else if (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (propertyJson.type == JsonData.Type.Array)
                            {
                                IList list = ListFromArray(propertyJson, memberType.GetGenericArguments()[0]);
                                SetMemberValue(member, dataclass, list);
                            }
                        }
                        else if (memberType.IsArray)
                        {
                            if (propertyJson.type == JsonData.Type.Array)
                            {
                                Array array = ArrayFromJson(propertyJson, memberType.GetElementType());
                                SetMemberValue(member, dataclass, array);
                            }
                        }
                        else if (propertyJson.type == JsonData.Type.Object)
                        {
                            SetMemberValue(member, dataclass, FromJson(propertyJson, memberType));
                        }
                        break;
                }
            }

            return dataclass;
        }

        private static List<MemberInfo> GetMembers(Type type)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            return type.GetFields(flags)
                .Select<FieldInfo, MemberInfo>(f => f)
                .Concat(type.GetProperties(flags))
                .Select(m => new
                {
                    Member = m,
                    JsonProperty = m.GetCustomAttribute<JsonProperty>()
                })
                .OrderByDescending(m => m.JsonProperty != null)
                .ThenBy(m => m.Member.Name)
                .Select(m => m.Member)
                .ToList();
        }

        private static Type GetMemberType(MemberInfo member)
        {
            if (member is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }

            if (member is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }

            return null;
        }

        private static void SetMemberValue(MemberInfo member, object instance, object value)
        {
            if (member is PropertyInfo propertyInfo)
            {
                propertyInfo.SetValue(instance, value);
            }
            else if (member is FieldInfo fieldInfo)
            {
                fieldInfo.SetValue(instance, value);
            }
        }

        private static Array ArrayFromJson(JsonData json, Type elementType)
        {
            IList list = ListFromArray(json, elementType);
            Array array = Array.CreateInstance(elementType, list.Count);
            list.CopyTo(array, 0);
            return array;
        }

        private static IList ListFromArray(JsonData json, Type elementType)
        {
            IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            if (json == null || json.array == null)
            {
                return list;
            }

            switch (Type.GetTypeCode(elementType))
            {
                case TypeCode.String:
                    foreach (var item in json.array)
                    {
                        string stringValue = item;
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
                        int.TryParse(item.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue);
                        list.Add(intValue);
                    }

                    break;
                case TypeCode.Int64:
                    foreach (var item in json.array)
                    {
                        long.TryParse(item.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longValue);
                        list.Add(longValue);
                    }

                    break;
                case TypeCode.Single:
                    foreach (var item in json.array)
                    {
                        float.TryParse(item.json, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue);
                        list.Add(floatValue);
                    }

                    break;
                case TypeCode.Double:
                    foreach (var item in json.array)
                    {
                        double.TryParse(item.json, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue);
                        list.Add(doubleValue);
                    }

                    break;
                case TypeCode.SByte:
                    foreach (var item in json.array)
                    {
                        sbyte.TryParse(item.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte sbyteValue);
                        list.Add(sbyteValue);
                    }

                    break;
                case TypeCode.Int16:
                    foreach (var item in json.array)
                    {
                        short.TryParse(item.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out short shortValue);
                        list.Add(shortValue);
                    }

                    break;
                case TypeCode.UInt32:
                    foreach (var item in json.array)
                    {
                        uint.TryParse(item.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint uintValue);
                        list.Add(uintValue);
                    }

                    break;
                case TypeCode.UInt64:
                    foreach (var item in json.array)
                    {
                        ulong.TryParse(item.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong ulongValue);
                        list.Add(ulongValue);
                    }

                    break;
                case TypeCode.UInt16:
                    foreach (var item in json.array)
                    {
                        ushort.TryParse(item.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort ushortValue);
                        list.Add(ushortValue);
                    }

                    break;
                case TypeCode.Decimal:
                    foreach (var item in json.array)
                    {
                        decimal.TryParse(JsonString.Unquote(item.json), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue);
                        list.Add(decimalValue);
                    }

                    break;
                case TypeCode.DateTime:
                    foreach (var item in json.array)
                    {
                        DateTime.TryParse(JsonString.Unquote(item.json), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dateTimeValue);
                        list.Add(dateTimeValue);
                    }

                    break;
                default:
                    foreach (var item in json.array)
                    {
                        if (item == null || item.type == JsonData.Type.None)
                        {
                            if (elementType.IsValueType)
                            {
                                list.Add(Activator.CreateInstance(elementType));
                            }
                            else
                            {
                                list.Add(null);
                            }
                        }
                        else
                        {
                            list.Add(FromJson(item, elementType));
                        }
                    }

                    break;
            }

            return list;
        }

        public static JsonData ProcessJson(string json, ref int index)
        {
            if (index < 0 || index >= json.Length)
            {
                return null;
            }

            SkipWhiteSpace(json, ref index);
            if (index >= json.Length)
            {
                return null;
            }

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
