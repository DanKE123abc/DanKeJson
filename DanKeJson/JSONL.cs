using System.Collections.Generic;
using System.IO;
using DanKeJson.Utils;

namespace DanKeJson
{
    public class JSONL
    {
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

        public static JsonData LineToData(string text, int lineNumber)
        {
            string jsonLine = null;
            JsonData dataLine = null;
            if (FilePathUtility.IsFilePath(text))
            {
                jsonLine = FileLineReader.ReadLine(text, lineNumber);
                dataLine = JSON.ToData(jsonLine);
            }
            return dataLine;
        }

        public static T LineToData<T>(string text, int lineNumber) where T : class, new()
        {
            string jsonLine = null;
            T dataLine = null;
            if (FilePathUtility.IsFilePath(text))
            {
                jsonLine = FileLineReader.ReadLine(text, lineNumber);
                dataLine = JSON.ToData<T>(jsonLine);
            }
            return dataLine;
        }

    }
}