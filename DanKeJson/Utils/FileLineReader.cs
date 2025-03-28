using System;
using System.Collections.Generic;
using System.IO;

namespace DanKeJson.Utils
{
    public class FileLineReader
    {
        public static string ReadLine(string filePath, int lineNumber)
        {
            try
            {
                // 检查行号是否有效
                if (lineNumber < 1)
                {
                    return null;
                }
                using (var reader = new StreamReader(filePath))
                {
                    int currentLine = 1;
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        if (currentLine == lineNumber)
                            return line;
                        currentLine++;
                    }
                }
                // 文件行数不足
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception ex)
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
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}