namespace DanKeJson.Utils
{
    /// <summary>
    /// TextUtility : 解析前的文本预处理。
    /// </summary>
    internal static class TextUtility
    {
        /// <summary>
        /// 去除文本开头的 UTF-8 BOM（\uFEFF），例如从 Windows 记事本保存的 JSON 文件。
        /// </summary>
        public static string StripLeadingBom(string text)
        {
            if (!string.IsNullOrEmpty(text) && text[0] == '\uFEFF')
            {
                return text.Substring(1);
            }

            return text;
        }
    }
}
