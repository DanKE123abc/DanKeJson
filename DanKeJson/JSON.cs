using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DanKeJson.Json;
using DanKeJson.Utils;

#pragma warning disable CS8603

namespace DanKeJson
{

    /// <summary>
    /// DanKeJson : Serialization and Deserialization
    /// </summary>
    public static class JSON
    {
        /// <summary>
        /// Serializing Json(String) to JsonData
        /// About Json : https://json.org
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text)
        {
            if (text == null)
            {
                return null;
            }

            text = TextUtility.StripLeadingBom(text);

            int index = 0;
            JsonData json = Deserializer.ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return json;
            }
            
            return null;
        }

        /// <summary>
        /// Serializing Json(String) to Class
        /// About Json : https://json.org
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        /// <remarks>
        /// 成员按名称精确匹配（区分大小写，不做忽略大小写的匹配），
        /// 且不做类型强转：JSON 字符串不会自动转换成数字或布尔成员，类型不匹配时成员保持默认值。
        /// 需要自定义键名时使用 <see cref="JsonProperty"/>。
        /// </remarks>
        public static T ToData<T>(string text) where T : class, new()
        {
            if (text == null)
            {
                return default(T);
            }

            text = TextUtility.StripLeadingBom(text);

            int index = 0;
            JsonData json = Deserializer.ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return (T)Deserializer.FromJson(json, typeof(T));
            }

            return default(T);
        }

        /// <summary>
        /// Read a Json file and parse it to JsonData
        /// </summary>
        /// <param name="filePath">the file path</param>
        /// <returns>JsonData</returns>
        public static JsonData ToDataFromFile(string filePath)
        {
            return ToData(File.ReadAllText(filePath));
        }

        /// <summary>
        /// Read a Json file and deserialize it to Class
        /// </summary>
        /// <param name="filePath">the file path</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        public static T ToDataFromFile<T>(string filePath) where T : class, new()
        {
            return ToData<T>(File.ReadAllText(filePath));
        }
        
        /// <summary>
        /// Deserializing JsonData to Json(String)
        /// About Json : https://json.org
        /// </summary>
        /// <param name="json">the JsonData</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(JsonData json)
        {
            if (json == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            Serializer.ProcessData(json, stringBuilder);
            return stringBuilder.ToString();
        }
        
        /// <summary>
        /// Deserializing Object to Json(String)
        /// About Json : https://json.org
        /// </summary>
        /// <param name="jsonObject">object instantiated by the class</param>
        /// <returns>Json(String)</returns>
        /// <remarks>
        /// NaN / Infinity 无法用 JSON 表示，按 JSON.stringify 的做法输出为 null
        /// （JSON5 支持这些字面量，JSON5.ToJson 会保留原样）。
        /// </remarks>
        public static string ToJson(object jsonObject)
        {
            if (jsonObject == null)
            {
                return null;
            }

            JsonData json = Serializer.FromObject(jsonObject);
            StringBuilder stringBuilder = new StringBuilder();
            Serializer.ProcessData(json, stringBuilder);
            return stringBuilder.ToString();
        }
        
    }
}
