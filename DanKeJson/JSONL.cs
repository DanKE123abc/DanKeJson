using System.Collections.Generic;
using System.IO;
using System.Text;
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
                    // 跳过无法解析的行（例如被换行拆开的美化 JSON），避免插入 null 元素
                    JsonData line = JSON.ToData(l);
                    if (line == null) continue;
                    dataLines.Add(line);
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
                    // 解析失败的行（或该行就是 null）会被跳过，避免插入 null 元素
                    T line = JSON.ToData<T>(l);
                    if (line == null) continue;
                    dataLines.Add(line);
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
                    return JSON.ToData(jsonLine);
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
                    return JSON.ToData<T>(jsonLine);
                }
            }
            return null;
        }
        
        
        /// <summary>
        /// Deserializing JsonData List to Json(String)
        /// </summary>
        /// <param name="jsonDataList">the JsonData list</param>
        /// <param name="filePath">the output file path</param>
        /// <returns></returns>
        /// <remarks>
        /// 指定 filePath 时按行写入：UTF-8 无 BOM、行分隔符为 \n（与返回值一致），
        /// 列表中的 null 元素写成 JSON 的 null；目标目录必须已存在，否则抛出 DirectoryNotFoundException。
        /// </remarks>
        public static string ListToJson(List<JsonData> jsonDataList, string filePath = null)
        {
            if (jsonDataList == null)
            {
                return null;
            }
            var jsonLines = new List<string>();
            foreach (var l in jsonDataList)
            {
                // 列表中允许出现 null 元素，写成 JSON 的 null
                jsonLines.Add(JSON.ToJson(l) ?? "null");
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                // 不带 BOM 的 UTF-8，行分隔符统一为 \n（与返回值一致）
                using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                {
                    writer.NewLine = "\n";
                    foreach (var line in jsonLines)
                    {
                        string escapedLine = line.Replace("\r\n", "\\n").Replace("\n", "\\n");
                        writer.WriteLine(escapedLine);
                    }
                }
            }
            return string.Join("\n", jsonLines);
        }
        
        
        /// <summary>
        /// Deserializing Object List to Json(String)
        /// </summary>
        /// <param name="jsonDataList">the JsonData list</param>
        /// <param name="filePath">the output file path</param>
        /// <typeparam name="T">Class</typeparam>
        /// <returns></returns>
        /// <remarks>
        /// 指定 filePath 时按行写入：UTF-8 无 BOM、行分隔符为 \n（与返回值一致），
        /// 列表中的 null 元素写成 JSON 的 null；目标目录必须已存在，否则抛出 DirectoryNotFoundException。
        /// </remarks>
        public static string ListToJson<T>(List<T> jsonDataList, string filePath = null) where T : class, new()
        {
            if (jsonDataList == null)
            {
                return null;
            }
            var jsonLines = new List<string>();
            foreach (var l in jsonDataList)
            {
                // 列表中允许出现 null 元素，写成 JSON 的 null
                jsonLines.Add(JSON.ToJson(l) ?? "null");
            }
            if (!string.IsNullOrEmpty(filePath))
            {
                // 不带 BOM 的 UTF-8，行分隔符统一为 \n（与返回值一致）
                using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(false)))
                {
                    writer.NewLine = "\n";
                    foreach (var line in jsonLines)
                    {
                        string escapedLine = line.Replace("\r\n", "\\n").Replace("\n", "\\n");
                        writer.WriteLine(escapedLine);
                    }
                }
            }
            return string.Join("\n", jsonLines);
        }

    }
}