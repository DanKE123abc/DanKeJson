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
#pragma warning disable CS8625
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
            DepthGuard.Enter();
            try
            {
                return FromJsonCore(json, type);
            }
            finally
            {
                DepthGuard.Exit();
            }
        }

        private static object FromJsonCore(JsonData json, Type type)
        {
            // JsonData 目标：保留原始节点（JsonData 没有无参构造，不能走 Activator）
            if (type == typeof(JsonData))
            {
                return json;
            }

            // 字典目标：顶层、成员、集合元素共用同一实现
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                return DictionaryFromJson(json, type);
            }

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
                Type valueType = Nullable.GetUnderlyingType(memberType) ?? memberType;
                bool isNullable = valueType != memberType;
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

                // JSON null：可空成员保持 null，不写入值类型的默认值
                if (isNullable && propertyJson.type == JsonData.Type.None)
                {
                    continue;
                }

                // 枚举：既支持数字，也支持名称（含 Flags 的逗号写法）
                if (valueType.IsEnum)
                {
                    if (TryParseEnum(propertyJson, valueType, out object enumValue))
                    {
                        SetMemberValue(member, dataclass, enumValue);
                    }

                    continue;
                }

                switch (Type.GetTypeCode(valueType))
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
                    case TypeCode.Byte:
                        byte.TryParse(propertyJson.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte byteValue);
                        SetMemberValue(member, dataclass, byteValue);
                        break;
                    case TypeCode.Char:
                        SetMemberValue(member, dataclass, CharFromJson(propertyJson));
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
                        if (valueType == typeof(TimeSpan))
                        {
                            TimeSpan.TryParse(JsonString.Unquote(propertyJson.json), CultureInfo.InvariantCulture, out TimeSpan timeSpanValue);
                            SetMemberValue(member, dataclass, timeSpanValue);
                        }
                        else if (valueType == typeof(Guid))
                        {
                            Guid.TryParse(JsonString.Unquote(propertyJson.json), out Guid guidValue);
                            SetMemberValue(member, dataclass, guidValue);
                        }
                        else if (valueType == typeof(DateTimeOffset))
                        {
                            DateTimeOffset.TryParse(JsonString.Unquote(propertyJson.json), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset dateTimeOffsetValue);
                            SetMemberValue(member, dataclass, dateTimeOffsetValue);
                        }
                        else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            if (propertyJson.type == JsonData.Type.Array)
                            {
                                IList list = ListFromArray(propertyJson, valueType.GetGenericArguments()[0]);
                                SetMemberValue(member, dataclass, list);
                            }
                        }
                        else if (valueType.IsArray)
                        {
                            if (propertyJson.type == JsonData.Type.Array)
                            {
                                Array array = ArrayFromJson(propertyJson, valueType.GetElementType());
                                SetMemberValue(member, dataclass, array);
                            }
                        }
                        else if (valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            if (propertyJson.type == JsonData.Type.Object)
                            {
                                SetMemberValue(member, dataclass, DictionaryFromJson(propertyJson, valueType));
                            }
                        }
                        else if (propertyJson.type == JsonData.Type.Object)
                        {
                            SetMemberValue(member, dataclass, FromJson(propertyJson, valueType));
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
                // 索引器（带参数的属性）无法直接 SetValue，必须排除
                .Concat(type.GetProperties(flags).Where(p => p.GetIndexParameters().Length == 0))
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

        /// <summary>
        /// 解析枚举成员：JSON 数字（按基础类型转换）与 JSON 字符串（名称，含 Flags 的逗号写法）都支持。
        /// </summary>
        private static bool TryParseEnum(JsonData json, Type enumType, out object value)
        {
            value = null;
            if (json == null)
            {
                return false;
            }

            if (json.type == JsonData.Type.String)
            {
                string text = JsonString.Unquote(json.json);
                if (string.IsNullOrEmpty(text))
                {
                    return false;
                }

                try
                {
                    value = Enum.Parse(enumType, text, true);
                    return true;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (OverflowException)
                {
                    return false;
                }
            }

            if (json.type == JsonData.Type.Number)
            {
                try
                {
                    object numeric = Convert.ChangeType(json.json, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture);
                    value = Enum.ToObject(enumType, numeric);
                    return true;
                }
                catch (FormatException)
                {
                    return false;
                }
                catch (OverflowException)
                {
                    return false;
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }

            return false;
        }

        private static char CharFromJson(JsonData json)
        {
            if (json == null)
            {
                return default;
            }

            if (json.type == JsonData.Type.Number)
            {
                return int.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out int code)
                    ? (char)code
                    : default;
            }

            string text = JsonString.Unquote(json.json);
            return string.IsNullOrEmpty(text) ? default : text[0];
        }

        /// <summary>
        /// 把单个 JsonData 值转换为指定类型，用于字典值等容器的元素转换。
        /// </summary>
        private static object ValueFromJson(JsonData json, Type type)
        {
            Type target = Nullable.GetUnderlyingType(type) ?? type;

            if (json == null || json.type == JsonData.Type.None)
            {
                return type.IsValueType && Nullable.GetUnderlyingType(type) == null
                    ? Activator.CreateInstance(type)
                    : null;
            }

            if (target.IsEnum)
            {
                return TryParseEnum(json, target, out object enumValue)
                    ? enumValue
                    : Activator.CreateInstance(target);
            }

            switch (Type.GetTypeCode(target))
            {
                case TypeCode.String:
                    return (string)json;
                case TypeCode.Boolean:
                    bool.TryParse(json.json, out bool boolValue);
                    return boolValue;
                case TypeCode.Char:
                    return CharFromJson(json);
                case TypeCode.Byte:
                    byte.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out byte byteValue);
                    return byteValue;
                case TypeCode.SByte:
                    sbyte.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out sbyte sbyteValue);
                    return sbyteValue;
                case TypeCode.Int16:
                    short.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out short shortValue);
                    return shortValue;
                case TypeCode.UInt16:
                    ushort.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out ushort ushortValue);
                    return ushortValue;
                case TypeCode.Int32:
                    int.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue);
                    return intValue;
                case TypeCode.UInt32:
                    uint.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out uint uintValue);
                    return uintValue;
                case TypeCode.Int64:
                    long.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out long longValue);
                    return longValue;
                case TypeCode.UInt64:
                    ulong.TryParse(json.json, NumberStyles.Integer, CultureInfo.InvariantCulture, out ulong ulongValue);
                    return ulongValue;
                case TypeCode.Single:
                    float.TryParse(json.json, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue);
                    return floatValue;
                case TypeCode.Double:
                    double.TryParse(json.json, NumberStyles.Float, CultureInfo.InvariantCulture, out double doubleValue);
                    return doubleValue;
                case TypeCode.Decimal:
                    decimal.TryParse(JsonString.Unquote(json.json), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decimalValue);
                    return decimalValue;
                case TypeCode.DateTime:
                    DateTime.TryParse(JsonString.Unquote(json.json), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime dateTimeValue);
                    return dateTimeValue;
                default:
                    if (target == typeof(Guid))
                    {
                        Guid.TryParse(JsonString.Unquote(json.json), out Guid guidValue);
                        return guidValue;
                    }

                    if (target == typeof(TimeSpan))
                    {
                        TimeSpan.TryParse(JsonString.Unquote(json.json), CultureInfo.InvariantCulture, out TimeSpan timeSpanValue);
                        return timeSpanValue;
                    }

                    if (target == typeof(DateTimeOffset))
                    {
                        DateTimeOffset.TryParse(JsonString.Unquote(json.json), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTimeOffset offsetValue);
                        return offsetValue;
                    }

                    if (json.type == JsonData.Type.Array)
                    {
                        if (target.IsArray)
                        {
                            return ArrayFromJson(json, target.GetElementType());
                        }

                        if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(List<>))
                        {
                            return ListFromArray(json, target.GetGenericArguments()[0]);
                        }
                    }

                    if (json.type == JsonData.Type.Object)
                    {
                        if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                        {
                            return DictionaryFromJson(json, target);
                        }

                        return FromJson(json, target);
                    }

                    return null;
            }
        }

        private static IDictionary DictionaryFromJson(JsonData json, Type dictionaryType)
        {
            IDictionary dictionary = (IDictionary)Activator.CreateInstance(dictionaryType);
            if (json == null || json.map == null)
            {
                return dictionary;
            }

            Type[] arguments = dictionaryType.GetGenericArguments();
            Type keyType = arguments[0];
            Type valueType = arguments[1];

            foreach (KeyValuePair<string, JsonData> pair in json.map)
            {
                if (!TryConvertKey(pair.Key, keyType, out object key))
                {
                    continue;
                }

                dictionary[key] = ValueFromJson(pair.Value, valueType);
            }

            return dictionary;
        }

        private static bool TryConvertKey(string key, Type keyType, out object value)
        {
            value = null;
            Type target = Nullable.GetUnderlyingType(keyType) ?? keyType;

            if (target == typeof(string))
            {
                value = key;
                return true;
            }

            if (key == null)
            {
                return false;
            }

            try
            {
                if (target.IsEnum)
                {
                    value = Enum.Parse(target, key, true);
                    return true;
                }

                if (target == typeof(Guid))
                {
                    value = Guid.Parse(key);
                    return true;
                }

                value = Convert.ChangeType(key, target, CultureInfo.InvariantCulture);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (InvalidCastException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
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
                        if (elementType == typeof(JsonData))
                        {
                            // JsonData 元素直接保留原始节点（含 None 节点）
                            list.Add(item);
                            continue;
                        }

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
                //Object；每进入一层容器计数一次，使 MaxDepth 与文档嵌套层级一致
                DepthGuard.Enter();
                try
                {
                    jsonData = ToObject(json, ref index);
                }
                finally
                {
                    DepthGuard.Exit();
                }
            }
            else if (cur == '[')
            {
                //Array
                DepthGuard.Enter();
                try
                {
                    jsonData = ToArray(json, ref index);
                }
                finally
                {
                    DepthGuard.Exit();
                }
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
