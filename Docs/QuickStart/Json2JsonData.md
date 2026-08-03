# 将 JSON 读取为 JsonData（反序列化）

[上一页：Hello DanKeJson](../DanKeJson.md)

## 快速入门

使用 `JSON.ToData(string)` 将一段 JSON 文本解析为 [JsonData](../API/JsonData.md)：

```csharp
using DanKeJson;

string text = "{\"name\":\"DanKe\",\"age\":25}";

JsonData json = JSON.ToData(text);

string name = json["name"];   // "DanKe"
int age = json["age"];        // 25
```

`JsonData` 提供了到 `string` / `bool` / `int` / `long` / `float` / `double` / `sbyte` / `short` / `uint` / `ulong` / `ushort` 的[隐式转换](../API/JsonData.md)，因此可以像上面这样直接赋值。

## 解析规则

- 键不存在时，`json["key"]` 返回 `null`，隐式转换为值类型时得到默认值（如 `0` / `false`）。
- 无法识别的值会被解析为 `null`（`JsonData.Type.None`）。
- 如果文本无法被完整解析，`ToData` 返回 `null`。

## 读取文件

`ToData` 只解析传入的文本。如需从文件读取，请使用 `ToDataFromFile`：

```csharp
JsonData json = JSON.ToDataFromFile("config.json");
```

## 解析 JSON5

[JSON5](../API/JSON5.md) 允许注释、单引号字符串、不带引号的键名以及多余逗号。使用 `JSON5.ToData` 解析：

```csharp
string json5 = """
    {
        // 注释
        name: 'DanKe',   // 键名无需引号，字符串可用单引号
        age: 25,
        tags: ['c#', 'json',],   // 允许多余逗号
    }
    """;

JsonData json = JSON5.ToData(json5);

string name = json["name"];   // "DanKe"
int age = json["age"];        // 25
```

> **注意**：注释是通过正则表达式（`CommentParser`）移除的，可能会影响解析性能。

## 读取 JSONL

[.jsonl（JSON Lines）](../API/JSONL.md) 文件中每行都是一条独立的 JSON。使用 `JSONL` 类读取：

```csharp
// 读取全部行
List<JsonData> lines = JSONL.AllLineToData("data.jsonl");

// 只读取第 2 行（行号从 1 开始）
JsonData second = JSONL.LineToData("data.jsonl", 2);
```

## 遍历对象与数组

```csharp
JsonData json = JSON.ToData("{\"items\":[1,2,3],\"flag\":true}");

// 判断键是否存在
bool hasFlag = json.HasKey("flag");   // true

// 判断类型
JsonData.Type t = json.type;

if (t == JsonData.Type.Object)
{
    foreach (var key in json.map.Keys)
    {
        Console.WriteLine(key + " = " + json[key]);
    }
}
else if (t == JsonData.Type.Array)
{
    for (int i = 0; i < json.array.Count; i++)
    {
        Console.WriteLine(json[i]);
    }
}
```

[下一页：将 JSON 读取为 .NET 对象](./Json2Object.md)
