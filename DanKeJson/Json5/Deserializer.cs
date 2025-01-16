using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using DanKeJson.Json;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson.Json5
{
    public class Deserializer
    {
        public static JsonData FromObject(object jsonObject)
        {
            return Json.Deserializer.FromObject(jsonObject);
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
        
    }
}