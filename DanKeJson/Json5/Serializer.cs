using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DanKeJson.Json;
using static DanKeJson.Json.Reader;
using static DanKeJson.Json5.ReaderExts;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson.Json5
{
    public class Serializer
    {
        public static object FromJson(JsonData json, Type type)
        {
            return Json.Serializer.FromJson(json, type);
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
            else if (cur == '\'')
            {
                //String (Single)
                jsonData = ToString_Single(json, ref index);
            }
            else if (cur == 't' || cur == 'f')
            {
                //Boolean
                jsonData = ToBoolean(json, ref index);
            }
            else if (cur == '-' || cur == '+' || char.IsDigit(cur) || cur == 'N' || cur == 'I')
            {
                //Number
                jsonData = ToNumber(json, ref index);
            }
            else if (cur == '{')
            {
                //Object
                jsonData = ReaderExts.ToObject(json, ref index);
            }
            else if (cur == '[')
            {
                //Array
                jsonData = ReaderExts.ToArray(json, ref index);
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