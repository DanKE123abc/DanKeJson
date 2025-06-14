using System.Collections.Generic;
using DanKeJson.Utils;

namespace DanKeJson
{
    public class JSONL
    {
        /// <summary>
        /// Serializing .jsonl File to List(JsonData)
        /// </summary>
        /// <param name="filePath">the jsonl file path</param>
        /// <returns>JsonData</returns>
        public static List<JsonData> AllLineToData(string filePath)
        {
            var dataLines = new List<JsonData>();
            if (FilePathUtility.IsFilePath(filePath))
            {
                var jsonLines = FileLineReader.ReadAllLines(filePath);
                foreach (var l in jsonLines)
                {
                    // 添加空行检查
                    if (string.IsNullOrWhiteSpace(l)) continue;
                    dataLines.Add(JSON.ToData(l, true));
                }
            }
            return dataLines;
        }

        /// <summary>
        /// Serializing .jsonl File to List(Class)
        /// </summary>
        /// <param name="filePath">the jsonl file path</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>JsonData</returns>
        public static List<T> AllLineToData<T>(string filePath) where T : class, new()
        {
            var dataLines = new List<T>();
            if (FilePathUtility.IsFilePath(filePath))
            {
                var jsonLines = FileLineReader.ReadAllLines(filePath);
                foreach (var l in jsonLines)
                {
                    // 添加空行检查
                    if (string.IsNullOrWhiteSpace(l)) continue;
                    dataLines.Add(JSON.ToData<T>(l, true));
                }
            }
            return dataLines;
        }

        /// <summary>
        /// Serializing .jsonl File to JsonData
        /// </summary>
        /// <param name="filePath">the jsonl file path</param>
        /// <param name="lineNumber">the line number</param>
        /// <returns>JsonData</returns>
        public static JsonData LineToData(string filePath, int lineNumber)
        {
            if (FilePathUtility.IsFilePath(filePath))
            {
                var jsonLine = FileLineReader.ReadLine(filePath, lineNumber);
                if (!string.IsNullOrWhiteSpace(jsonLine))
                {
                    return JSON.ToData(jsonLine, true);
                }
            }
            return null;
        }

        /// <summary>
        /// Serializing .jsonl File to Class
        /// </summary>
        /// <param name="filePath">the jsonl file path</param>
        /// <param name="lineNumber">the line number</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns>JsonData</returns>
        public static T LineToData<T>(string filePath, int lineNumber) where T : class, new()
        {
            if (FilePathUtility.IsFilePath(filePath))
            {
                var jsonLine = FileLineReader.ReadLine(filePath, lineNumber);
                if (!string.IsNullOrWhiteSpace(jsonLine))
                {
                    return JSON.ToData<T>(jsonLine, true);
                }
            }
            return null;
        }
        
        /// <summary>
        /// Deserializing JsonData List to Json(String)
        /// </summary>
        /// <param name="jsonDataList">the JsonData list</param>
        /// <returns></returns>
        public static string ListToJson(List<JsonData> jsonDataList)
        {
            if (jsonDataList == null)
            {
                return null;
            }
            var jsonLines = new List<string>();
            foreach (var l in jsonDataList)
            {
                jsonLines.Add(JSON.ToJson(l));
            }
            return string.Join("\n", jsonLines);
        }
        
        
        /// <summary>
        /// Deserializing Object List to Json(String)
        /// </summary>
        /// <param name="jsonDataList">the JsonData list</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns></returns>
        public static string ListToJson<T>(List<T> jsonDataList) where T : class, new()
        {
            if (jsonDataList == null)
            {
                return null;
            }
            var jsonLines = new List<string>();
            foreach (var l in jsonDataList)
            {
                jsonLines.Add(JSON.ToJson(l));
            }
            return string.Join("\n", jsonLines);
        }

    }
}