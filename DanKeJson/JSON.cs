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
