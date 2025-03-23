using System.Collections.Generic;
using System.IO;
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
            var jsonLines = new List<string>();
            var dataLines = new List<JsonData>();
            if (FilePathUtility.IsFilePath(filePath))
            {
                jsonLines = FileLineReader.ReadAllLines(filePath);
                foreach (var l in jsonLines)
                {
                    dataLines.Add(JSON.ToData(l));
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
            var jsonLines = new List<string>();
            var dataLines = new List<T>();
            if (FilePathUtility.IsFilePath(filePath))
            {
                jsonLines = FileLineReader.ReadAllLines(filePath);
                foreach (var l in jsonLines)
                {
                    dataLines.Add(JSON.ToData<T>(l));
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
            string jsonLine = null;
            JsonData dataLine = null;
            if (FilePathUtility.IsFilePath(filePath))
            {
                jsonLine = FileLineReader.ReadLine(filePath, lineNumber);
                dataLine = JSON.ToData(jsonLine);
            }
            return dataLine;
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
            string jsonLine = null;
            T dataLine = null;
            if (FilePathUtility.IsFilePath(filePath))
            {
                jsonLine = FileLineReader.ReadLine(filePath, lineNumber);
                dataLine = JSON.ToData<T>(jsonLine);
            }
            return dataLine;
        }

    }
}