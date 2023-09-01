using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DanKeJson
{
    public static class CommentParser
    {
        public static string RemoveComments(string json)
        {
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

