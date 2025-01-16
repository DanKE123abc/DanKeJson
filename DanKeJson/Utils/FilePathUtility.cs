using System.IO;

namespace DanKeJson.Utils
{
    public class FilePathUtility
    {
        public static bool IsFilePath(string path)
        {
            if (string.IsNullOrEmpty(Path.GetExtension(path)))
            {
                return false;
            }
            else
            {
                if (!File.Exists(path))
                {
                    return false;
                }
            }
            return true;
        }   
    }
}