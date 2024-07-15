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


Person person = new Person
{
  Name = "张三",
  Age = 30,
  IsMarried = true
};

Json5Config myconfig = new Json5Config
{
  AddCommaForObject = true,
  AddCommaForArray = false,
  StringQuoteStyle = Json5Config.StringQuoteType.SingleQuote,
  KeyNameStyle = Json5Config.KeyNameType.WithoutQuotes,
};

string jsonString = JSON5.ToJson(person, myconfig);
JsonData json = JSON5.ToData(jsonString);

Console.WriteLine(jsonString);

public class Person
{
  public string Name { get; set; }
  public int Age { get; set; }
  public bool IsMarried { get; set; }
}