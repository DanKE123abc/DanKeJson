# 将 JsonData 编写为 JSON（序列化）

[上一页：将 JSON 读取为 .NET 对象](./Json2Object.md)

## 快速入门

使用 `JSON.ToJson(JsonData)` 将 [JsonData](../API/JsonData.md) 序列化为 JSON 字符串：

```csharp
using DanKeJson;

JsonData json = JSON.ToData("{\"name\":\"DanKe\",\"age\":25}");

string text = JSON.ToJson(json);
Console.WriteLine(text);   // {"name":"DanKe","age":25}
```

传入 `null` 时返回 `null`。

## 手动构造 JsonData 再序列化

`JsonData` 支持通过隐式转换与索引器快速构造对象和数组：

```csharp
using DanKeJson;

// 构造一个对象
JsonData obj = new JsonData(JsonData.Type.Object);
obj["name"] = "DanKe";
obj["age"] = 25;
obj["isVip"] = true;

// 构造一个数组
JsonData arr = new JsonData(JsonData.Type.Array);
arr.Add("c#");
arr.Add("json");

// 把数组放进对象里
obj["skills"] = arr;

string text = JSON.ToJson(obj);
Console.WriteLine(text);
// {"name":"DanKe","age":25,"isVip":true,"skills":["c#","json"]}
```

> `obj["key"] = value` 会创建或覆盖键；`Add` 用于向数组追加元素。基本类型都可以通过隐式转换赋值，无需手动包装。

## 序列化为 JSON5

使用 `JSON5.ToJson(JsonData)` 序列化为 JSON5 字符串，并可通过 `Json5Options` 定制格式：

```csharp
using DanKeJson;

JsonData json = JSON.ToData("{\"name\":\"DanKe\",\"age\":25}");

var options = new Json5Options
{
    // 键名不加引号
    KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes,
    // 字符串使用单引号
    StringQuoteStyle = Json5Options.StringQuoteType.SingleQuote,
    // 对象、数组添加末尾逗号
    AddTailingCommaForObject = true,
    AddTailingCommaForArray = true
};

string json5 = JSON5.ToJson(json, options);
Console.WriteLine(json5);
// {name:'DanKe',age:25,}
```

`Json5Options` 的所有选项：

| 属性                      | 类型                          | 默认值       | 说明                   |
| :------------------------ | :---------------------------- | :----------- | :--------------------- |
| `KeyNameStyle`            | `KeyNameType`                 | `WithQuotes` | 键名是否加引号         |
| `StringQuoteStyle`        | `StringQuoteType`             | `DoubleQuote`| 字符串使用单/双引号    |
| `AddTailingCommaForObject`| `bool`                        | `false`      | 对象是否添加末尾逗号   |
| `AddTailingCommaForArray` | `bool`                        | `false`      | 数组是否添加末尾逗号   |

不传 `options`（或传 `null`）时使用默认值，输出与标准 JSON 一致：

```csharp
string json5 = JSON5.ToJson(json);
// {"name":"DanKe","age":25}
```

[下一页：将 .NET 对象编写为 JSON](./Object2Json.md)
