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
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text, bool skipFileCheck = false)
        {
            if (!skipFileCheck)
            {
                if (FilePathUtility.IsFilePath(text))
                {
                    text = File.ReadAllText(text);
                }
            }
            
            int index = 0;
            JsonData json = Serializer.ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return json;
            }
            
            return null;
        }

        /// <summary>
        /// Serializing Json(String) to Class
        /// About Json : https://json.org
        /// Using comments can affect performance
        /// </summary>
        /// <param name="text">the JsonText</param>
        /// <param name="skipFileCheck">Skip the file path check</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        public static T ToData<T>(string text, bool skipFileCheck = false) where T : class, new()
        {
            if (!skipFileCheck)
            {
                if (FilePathUtility.IsFilePath(text))
                {
                    text = File.ReadAllText(text);
                }
            }

            int index = 0;
            JsonData json = Serializer.ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return (T)Serializer.FromJson(json, typeof(T));
            }

            return default(T);
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
            Deserializer.ProcessData(json, stringBuilder);
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

            JsonData json = Deserializer.FromObject(jsonObject);
            StringBuilder stringBuilder = new StringBuilder();
            Deserializer.ProcessData(json, stringBuilder);
            return stringBuilder.ToString();
        }
        
    }
}
