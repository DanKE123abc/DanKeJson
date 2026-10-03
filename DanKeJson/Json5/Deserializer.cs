using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DanKeJson.Json;
using DanKeJson.Utils;
using static DanKeJson.Json.Reader;
using static DanKeJson.Json5.ReaderExts;

#pragma warning disable CS8604
#pragma warning disable CS8603
#pragma warning disable CS8602
#pragma warning disable CS8600
#pragma warning disable CS1591

namespace DanKeJson.Json5
{
    /// <summary>
    /// Deserializer : parse Json5 text into JsonData / .NET objects.
    /// </summary>
    public class Deserializer
    {
        public static object FromJson(JsonData json, Type type)
        {
            return Json.Deserializer.FromJson(json, type);
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
                //String (Double/Standard)，JSON5 额外支持 \x、\v、\0、续行
                jsonData = ToString_Double(json, ref index, true);
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
            else if (cur == '-' || cur == '+' || cur == '.' || char.IsDigit(cur) || cur == 'N' || cur == 'I')
            {
                //Number（JSON5 额外支持 0x / 0X 十六进制、.5 / 5. 写法）
                jsonData = ToNumberHex(json, ref index);
                if (jsonData == null)
                {
                    jsonData = ToNumber(json, ref index, true);
                }
            }
            else if (cur == '{')
            {
                //Object；每进入一层容器计数一次，使 MaxDepth 与文档嵌套层级一致
                DepthGuard.Enter();
                try
                {
                    jsonData = ReaderExts.ToObject(json, ref index);
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
                    jsonData = ReaderExts.ToArray(json, ref index);
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