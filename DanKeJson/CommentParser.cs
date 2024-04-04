#region Header

/*
* MIT License

Copyright (c) 2023 - present DanKe

* DanKeJson
a simple Json library for the .Net
JSON : https://json.org
 
* About Author
Name : DanKe
Address : Guangzhou City , Guangdong Provice , China
Mail : danke1024@foxmail.com
Github : https://github.com/DanKE123abc
  */

#endregion

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DanKeJson
{
    /// <summary>
    /// CommentParser
    /// </summary>
    public static class CommentParser
    {
        /// <summary>
        /// RemoveComments
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string RemoveComments(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return json;
            }
            
            json = Regex.Replace(json, @"//.*?(?=\r|\n|$)|"".*?""|'.*?'", match =>
            {
                if (match.Value.StartsWith("\"") || match.Value.StartsWith("'"))
                {
                    return match.Value;
                }
                else
                {
                    return "";
                }
            }, RegexOptions.Singleline);
            json = Regex.Replace(json, @"/\*[\s\S]*?\*/", "");

            return json;
        }

    }
}

