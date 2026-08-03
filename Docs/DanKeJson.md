# DanKeJson

JSON (J ava S cript O bject N otation) is a simple, yet powerful notation to specify data. It defines simple scalar types such as boolean, number (integers and reals) and string, and a couple of data structures: arrays (lists) and objects (dictionaries). For more information on the JSON format, visit [JSON.org](http://www.json.org/).

JSON5 (JSON for Humans) is an extension to the popular JSON file format that aims to be easier to **write and maintain \*by hand\* (e.g. for config files)**. It is *not intended* to be used for machine-to-machine communication. For more information on the JSON5 format, visit [JSON5.org](http://www.json5.org/).

DanKeJson is a simple Json library for the .Net.

## 目录

- [Hello DanKeJson](#hello-dankejson)
- 快速开始
  - [将 JSON 读取为 JsonData](./QuickStart/Json2JsonData.md)
  - [将 JSON 读取为 .NET 对象](./QuickStart/Json2Object.md)
  - [将 JsonData 编写为 JSON](./QuickStart/JsonData2Json.md)
  - [将 .NET 对象编写为 JSON](./QuickStart/Object2Json.md)
- API
  - [JSON](./API/JSON.md)
  - [JSON5](./API/JSON5.md)
  - [JSONL](./API/JSONL.md)
  - [JsonData](./API/JsonData.md)
  - [JsonData.Type](./API/JsonData.Type.md)

## Hello DanKeJson

下面是最简单的使用示例。完整示例请参考上方「快速开始」中的各篇文档。

```csharp
using DanKeJson;

// 1. 字符串 -> JsonData（反序列化）
string text = "{\"name\":\"DanKe\",\"age\":25}";
JsonData data = JSON.ToData(text);
Console.WriteLine(data["name"]);   // DanKe
Console.WriteLine(data["age"]);    // 25

// 2. 字符串 -> 实体类（反序列化）
User user = JSON.ToData<User>(text);

// 3. JsonData -> 字符串（序列化）
string json = JSON.ToJson(data);

// 4. 实体类 -> 字符串（序列化）
string json2 = JSON.ToJson(user);

// 5. JSON5（注释、单引号、无引号键名、多余逗号）
JsonData data5 = JSON5.ToData("{ name: 'DanKe', age: 25, }");
string json5 = JSON5.ToJson(data5, new Json5Options
{
    KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes,
    StringQuoteStyle = Json5Options.StringQuoteType.SingleQuote
});

// 6. JSONL（.jsonl 文件）
List<JsonData> lines = JSONL.AllLineToData("data.jsonl");

public class User
{
    public string name { get; set; }
    public int age { get; set; }
}
```

## License

```
MIT License

Copyright (c) 2023 - present DanKe

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```
