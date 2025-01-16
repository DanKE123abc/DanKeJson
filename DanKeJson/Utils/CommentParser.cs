using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DanKeJson.Utils
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

