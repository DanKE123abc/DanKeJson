using System.Diagnostics;
using DanKeJson;

string jsonText = @"
{
  test1: ""双引号文本"",
  test2 : [""数组1"", '数组2',],
  test3: '单引号文本',
}";

Stopwatch stopwatch = new Stopwatch();
stopwatch.Start();
DanKeJson.JsonData data = JSON.ToData(jsonText);
stopwatch.Stop();
Console.WriteLine($"JSON 解析完成，用时：{stopwatch.ElapsedMilliseconds} 毫秒");