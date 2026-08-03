using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DanKeJson.Json5;
using DanKeJson.Utils;

#pragma warning disable CS8603
#pragma warning disable CS8625

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
        /// <returns>JsonData</returns>
        public static JsonData ToData(string text)
        {
            if (text == null)
            {
                return null;
            }

            text = CommentParser.RemoveComments(text);
            int index = 0;
            JsonData json = Deserializer.ProcessJson(text, ref index);
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
        /// <typeparam name="T">Class</typeparam>
        /// <returns>T Class</returns>
        public static T ToData<T>(string text) where T : class, new()
        {
            if (text == null)
            {
                return default(T);
            }

            text = CommentParser.RemoveComments(text);
            int index = 0;
            JsonData json = Deserializer.ProcessJson(text, ref index);
            if (index == text.Length)
            {
                return (T)Deserializer.FromJson(json, typeof(T));
            }

            return default(T);
        }

        /// <summary>
        /// Read a Json5 file and parse it to JsonData
        /// </summary>
        /// <param name="filePath">the file path</param>
        /// <returns>JsonData</returns>
        public static JsonData ToDataFromFile(string filePath)
        {
            return ToData(File.ReadAllText(filePath));
        }

        /// <summary>
        /// Read a Json5 file and deserialize it to Class
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
            Serializer.ProcessData(json, stringBuilder, options);
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

            JsonData json = Serializer.FromObject(jsonObject);
            StringBuilder stringBuilder = new StringBuilder();
            Serializer.ProcessData(json, stringBuilder, options);
            return stringBuilder.ToString();
        }
        
    }


}
