using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DanKeJson.Json;
using DanKeJson.Utils;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson.Json5
{
    /// <summary>
    /// Serializer : serialize JsonData / .NET objects into Json5 text.
    /// </summary>
    public class Serializer
    {
        public static JsonData FromObject(object jsonObject)
        {
            return Json.Serializer.FromObject(jsonObject);
        }
        
        public static void ProcessData(JsonData json, StringBuilder builder, Json5Options options)
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
                    string content = JsonString.Unquote(json.json);
                    switch (options.StringQuoteStyle)
                    {
                        case Json5Options.StringQuoteType.DoubleQuote:
                            builder.Append('"');
                            builder.Append(JsonString.Escape(content));
                            builder.Append('"');
                            break;
                        case Json5Options.StringQuoteType.SingleQuote:
                            builder.Append('\'');
                            builder.Append(JsonString.EscapeSingleQuote(content));
                            builder.Append('\'');
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
                    bool firstObject = true;
                    foreach (var key in json.map.Keys)
                    {
                        if (!firstObject)
                        {
                            builder.Append(',');
                        }

                        firstObject = false;
                        switch (options.KeyNameStyle)
                        {
                            case Json5Options.KeyNameType.WithQuotes:
                                builder.Append('"');
                                builder.Append(JsonString.Escape(key));
                                builder.Append("\":");
                                break;
                            case Json5Options.KeyNameType.WithoutQuotes:
                                if (IsIdentifier(key))
                                {
                                    builder.Append(key);
                                    builder.Append(':');
                                }
                                else
                                {
                                    builder.Append('"');
                                    builder.Append(JsonString.Escape(key));
                                    builder.Append("\":");
                                }
                                break;
                        }
                        ProcessData(json[key], builder, options);
                    }

                    if (options.AddTailingCommaForObject && json.map.Count > 0)
                    {
                        builder.Append(',');
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
                        ProcessData(item, builder, options);
                    }

                    if (options.AddTailingCommaForArray && json.array.Count > 0)
                    {
                        builder.Append(',');
                    }
                    builder.Append(']');
                    break;

            }
        }

        /// <summary>
        /// Returns true if the key can be emitted without quotes (letters, digits, underscore),
        /// matching the characters the JSON5 parser accepts for unquoted keys.
        /// </summary>
        private static bool IsIdentifier(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            foreach (char c in key)
            {
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    return false;
                }
            }

            return true;
        }
        
    }
}