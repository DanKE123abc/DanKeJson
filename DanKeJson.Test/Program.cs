using System.Diagnostics;
using DanKeJson;

string jsonText = @"
{
  ""name"": ""张三"",
  ""age"": 30,
  ""city"": ""北京"",
  ""has_children"": false,
  ""hobbies"": [""阅读"", ""旅游"", ""编程""],
  ""address"": {
    ""street"": ""长安街"",
    ""number"": 123,
    ""postal_code"": ""100000""
  },
  ""email"": ""zhangsan@example.com"",
}";

DanKeJson.JsonData data = JSON.ToData(jsonText);
Console.WriteLine("");