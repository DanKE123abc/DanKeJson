using System;
using System.IO;

namespace DanKeJson.Tests
{
    /// <summary>为需要真实文件的测试提供一个随用随删的临时目录。</summary>
    public sealed class TempWorkspace : IDisposable
    {
        public string Root { get; }

        public TempWorkspace()
        {
            Root = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                "dankejson-tests-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Root);
        }

        public string PathOf(string fileName)
        {
            return System.IO.Path.Combine(Root, fileName);
        }

        public string WriteFile(string fileName, string content)
        {
            string path = PathOf(fileName);
            File.WriteAllText(path, content);
            return path;
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(Root))
                {
                    Directory.Delete(Root, true);
                }
            }
            catch
            {
                // 清理失败不影响测试结果
            }
        }
    }
}
