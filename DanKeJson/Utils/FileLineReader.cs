using System;
using System.Collections.Generic;
using System.IO;

#pragma warning disable CS8603

namespace DanKeJson.Utils
{
    public class FileLineReader
    {
        public static string ReadLine(string filePath, int lineNumber)
        {
            try
            {
                if (lineNumber < 1 || !File.Exists(filePath))
                {
                    return null;
                }
                using var reader = new StreamReader(filePath);
                for (int currentLine = 1; !reader.EndOfStream; currentLine++)
                {
                    var line = reader.ReadLine();
                    if (currentLine == lineNumber)
                        return line;
                }
                return null;
                // 文件行数不足
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch
            {
                return null;
            }
        }
        
        public static List<string> ReadAllLines(string filePath)
        {
            try
            {
                // 一次性读取所有行并转换为 List<string>
                List<string> lines = new List<string>(File.ReadAllLines(filePath));
                return lines;
            }
            catch (FileNotFoundException)
            {
                return new List<string>();
            }
            catch {
                return new List<string>();
            }
        }
    }
}