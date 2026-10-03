using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using DanKeJson.Utils;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS8601
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
            return FromObject(jsonObject, null);
        }

        private static JsonData FromObject(object jsonObject, HashSet<object> stack)
        {
            DepthGuard.Enter();
            try
            {
                return FromObjectCore(jsonObject, stack);
            }
            finally
            {
                DepthGuard.Exit();
            }
        }

        private static JsonData FromObjectCore(object jsonObject, HashSet<object> stack)
        {
            if (jsonObject == null)
            {
                return new JsonData(JsonData.Type.None);
            }

            System.Type type = jsonObject.GetType();

            // 数组 / 列表（List<T>、int[]、ArrayList 等）
            if (jsonObject is IList list)
            {
                JsonData arrayJson = new JsonData(JsonData.Type.Array);
                foreach (object item in list)
                {
                    arrayJson.array.Add(item == null
                        ? new JsonData(JsonData.Type.None)
                        : FromObject(item, stack));
                }

                return arrayJson;
            }

            // 字典
            if (jsonObject is IDictionary dictionary)
            {
                JsonData objectJson = new JsonData(JsonData.Type.Object);
                foreach (DictionaryEntry entry in dictionary)
                {
                    // JSON 的键必须是字符串；Hashtable 允许 null 键，这里统一写成空键
                    string entryKey = entry.Key == null ? string.Empty : entry.Key.ToString();
                    objectJson[entryKey] = entry.Value == null
                        ? new JsonData(JsonData.Type.None)
                        : FromObject(entry.Value, stack);
                }

                return objectJson;
            }

            // 其它可枚举类型（Stack / Queue / HashSet / LINQ 结果等）按数组序列化，
            // 否则会退化成 {"Count":n,...} 这种错误结果。
            // string 同样是 IEnumerable，但必须留给下面的字符串分支。
            if (jsonObject is IEnumerable enumerable && !(jsonObject is string))
            {
                JsonData enumerableJson = new JsonData(JsonData.Type.Array);
                foreach (object item in enumerable)
                {
                    enumerableJson.array.Add(item == null
                        ? new JsonData(JsonData.Type.None)
                        : FromObject(item, stack));
                }

                return enumerableJson;
            }

            // 枚举必须早于 Type.GetTypeCode 判断：枚举的 TypeCode 是其基础类型，
            // 否则会落到整数分支并输出成员名（如 Blue），产生非法 JSON。
            if (type.IsEnum)
            {
                object underlyingValue = Convert.ChangeType(jsonObject, Enum.GetUnderlyingType(type), CultureInfo.InvariantCulture);
                return new JsonData(JsonData.Type.Number)
                    { json = Convert.ToString(underlyingValue, CultureInfo.InvariantCulture) };
            }

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.String:
                case TypeCode.Char:
                    return new JsonData(JsonData.Type.String)
                        { json = "\"" + jsonObject + "\"" };
                case TypeCode.Boolean:
                    return new JsonData(JsonData.Type.Boolean)
                        { json = ((bool)jsonObject).ToString(CultureInfo.InvariantCulture).ToLower() };
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return new JsonData(JsonData.Type.Number)
                        { json = jsonObject.ToString() };
                case TypeCode.Single:
                    return new JsonData(JsonData.Type.Number)
                        { json = ((float)jsonObject).ToString(CultureInfo.InvariantCulture) };
                case TypeCode.Double:
                    return new JsonData(JsonData.Type.Number)
                        { json = ((double)jsonObject).ToString(CultureInfo.InvariantCulture) };
                case TypeCode.Decimal:
                    return new JsonData(JsonData.Type.Number)
                        { json = ((decimal)jsonObject).ToString(CultureInfo.InvariantCulture) };
                case TypeCode.DateTime:
                    return new JsonData(JsonData.Type.String)
                        { json = "\"" + ((DateTime)jsonObject).ToString("o", CultureInfo.InvariantCulture) + "\"" };
            }

            if (type == typeof(Guid))
            {
                return new JsonData(JsonData.Type.String) { json = "\"" + jsonObject + "\"" };
            }

            if (type == typeof(TimeSpan))
            {
                return new JsonData(JsonData.Type.String)
                    { json = "\"" + ((TimeSpan)jsonObject).ToString("c", CultureInfo.InvariantCulture) + "\"" };
            }

            if (type == typeof(DateTimeOffset))
            {
                return new JsonData(JsonData.Type.String)
                    { json = "\"" + ((DateTimeOffset)jsonObject).ToString("o", CultureInfo.InvariantCulture) + "\"" };
            }

            if (type == typeof(JsonData))
            {
                return (JsonData)jsonObject;
            }

            // 循环引用检测（仅引用类型）
            bool referenceType = !type.IsValueType;
            if (referenceType)
            {
                if (stack == null)
                {
                    stack = new HashSet<object>(ReferenceComparer.Instance);
                }

                if (!stack.Add(jsonObject))
                {
                    throw new InvalidOperationException(
                        "Detected a circular reference while serializing type " + type.FullName +
                        ". JSON does not support object cycles.");
                }
            }

            JsonData json = new JsonData(JsonData.Type.Object);
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (MemberInfo member in type.GetFields(flags)
                .Select<FieldInfo, MemberInfo>(f => f)
                .Concat(type.GetProperties(flags).Where(p => p.GetIndexParameters().Length == 0)))
            {
                object memberValue;
                Type memberType;
                string memberName = member.Name;

                if (member is PropertyInfo propertyInfo)
                {
                    if (!propertyInfo.CanRead)
                    {
                        continue;
                    }

                    memberValue = propertyInfo.GetValue(jsonObject);
                    memberType = propertyInfo.PropertyType;
                }
                else if (member is FieldInfo fieldInfo)
                {
                    memberValue = fieldInfo.GetValue(jsonObject);
                    memberType = fieldInfo.FieldType;
                }
                else
                {
                    continue;
                }

                // 获取自定义特性的名称
                var jsonProperty = member.GetCustomAttribute<JsonProperty>();
                if (jsonProperty != null)
                {
                    memberName = jsonProperty.Name;
                }

                if (memberValue == null)
                {
                    json[memberName] = new JsonData(JsonData.Type.None);
                    continue;
                }

                if (memberType.IsEnum)
                {
                    object underlying = Convert.ChangeType(memberValue, Enum.GetUnderlyingType(memberType), CultureInfo.InvariantCulture);
                    json[memberName] = new JsonData(JsonData.Type.Number) { json = underlying.ToString() };
                    continue;
                }

                switch (Type.GetTypeCode(memberType))
                {
                    case TypeCode.String:
                        json[memberName] = new JsonData(JsonData.Type.String)
                            { json = "\"" + memberValue.ToString() + "\"" };
                        break;
                    case TypeCode.Boolean:
                        json[memberName] = new JsonData(JsonData.Type.Boolean)
                            { json = memberValue.ToString().ToLower() };
                        break;
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                    case TypeCode.UInt16:
                        json[memberName] = new JsonData(JsonData.Type.Number)
                            { json = memberValue.ToString() };
                        break;
                    case TypeCode.Single:
                        json[memberName] = new JsonData(JsonData.Type.Number)
                            { json = ((float)memberValue).ToString(CultureInfo.InvariantCulture) };
                        break;
                    case TypeCode.Double:
                        json[memberName] = new JsonData(JsonData.Type.Number)
                            { json = ((double)memberValue).ToString(CultureInfo.InvariantCulture) };
                        break;
                    default:
                        json[memberName] = FromObject(memberValue, stack);
                        break;
                }
            }

            if (referenceType)
            {
                stack.Remove(jsonObject);
            }

            return json;
        }

        private sealed class ReferenceComparer : IEqualityComparer<object>
        {
            public static readonly ReferenceComparer Instance = new ReferenceComparer();

            public new bool Equals(object x, object y)
            {
                return ReferenceEquals(x, y);
            }

            public int GetHashCode(object obj)
            {
                return System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(obj);
            }
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