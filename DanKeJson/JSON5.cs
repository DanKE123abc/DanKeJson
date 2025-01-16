using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Linq;
using DanKeJson.Json5;
using DanKeJson.Utils;

namespace DanKeJson
{
    public class Json5Options
    {
        public enum KeyNameType
        {
            WithQuotes,
            WithoutQuotes
        }

        public enum StringQuoteType
        {
            SingleQuote,
            DoubleQuote
        }
        
        public bool AddTailingCommaForObject { get; set; } = false;
        
        public bool AddTailingCommaForArray { get; set; } = false;
        
        public KeyNameType KeyNameStyle { get; set; } = KeyNameType.WithQuotes;
        public StringQuoteType StringQuoteStyle { get; set; } = StringQuoteType.DoubleQuote;
        
    }

    /// <summary>
    /// DanKeJson : Serialization and Deserialization
    /// </summary>
    public static class JSON5
    {

        /// <summary>
        /// Serializing Json5(String) to JsonData
        /// About Json5 : https://json5.org
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
            text = CommentParser.RemoveComments(text);
            int index = 0;
            JsonData json = Serializer.ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return json;
            }
            
            return null;
        }

        /// <summary>
        /// Serializing Json5(String) to Class
        /// About Json5 : https://json5.org
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
            text = CommentParser.RemoveComments(text);
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
        /// About Json5 : https://json5.org
        /// </summary>
        /// <param name="json">the JsonData</param>
        /// <param name="options">JSON 5 format preferences</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(JsonData json, Json5Options options = null)
        {
            if (json == null)
            {
                return null;
            }
            
            if (options == null)
            {
                options = new Json5Options();
            }
            
            StringBuilder stringBuilder = new StringBuilder();
            Deserializer.ProcessData(json, stringBuilder, options);
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deserializing Object to Json(String)
        /// About Json5 : https://json5.org
        /// </summary>
        /// <param name="jsonObject">object instantiated by the class</param>
        /// <param name="options">JSON 5 format preferences</param>
        /// <returns>Json(String)</returns>
        public static string ToJson(object jsonObject, Json5Options options = null)
        {
            if (jsonObject == null)
            {
                return null;
            }

            if (options == null)
            {
                options = new Json5Options();
            }

            JsonData json = Deserializer.FromObject(jsonObject);
            StringBuilder stringBuilder = new StringBuilder();
            Deserializer.ProcessData(json, stringBuilder, options);
            return stringBuilder.ToString();
        }
        
    }


}