# 将 JSON 读取为 .NET 对象（反序列化）

[上一页：将 JSON 读取为 JsonData](./Json2JsonData.md)

## 快速入门

使用 `JSON.ToData<T>(string)` 将 JSON 文本反序列化为实体类：

```csharp
using DanKeJson;

public class User
{
    public string name { get; set; }
    public int age { get; set; }
    public bool isVip { get; set; }
}

string text = """
    {
        "name": "DanKe",
        "age": 25,
        "isVip": true
    }
    """;

User user = JSON.ToData<User>(text);

Console.WriteLine(user.name);   // DanKe
Console.WriteLine(user.age);    // 25
Console.WriteLine(user.isVip);  // True
```

> **注意**：`T` 必须是一个带有无参构造函数的 `class`（`where T : class, new()`）。

## 支持的类型

反序列化支持以下属性类型：

| 类型                                | 说明                                        |
| :---------------------------------- | :------------------------------------------ |
| `string`                            | JSON 字符串                                 |
| `bool`                              | JSON 布尔值                                 |
| 数字类型                             | `int` / `long` / `float` / `double` / `sbyte` / `short` / `uint` / `ulong` / `ushort` |
| `List<T>`                           | JSON 数组，`T` 可以是基本类型或自定义类      |
| `JsonData`                          | 原始 [JsonData](../API/JsonData.md)         |

## 键名映射（JsonProperty）

当 JSON 的键名与属性名不一致时，使用 `[JsonProperty("...")]` 特性指定 JSON 中的键名：

```csharp
using DanKeJson;

public class User
{
    [JsonProperty("user_name")]
    public string UserName { get; set; }

    [JsonProperty("user_age")]
    public int UserAge { get; set; }
}

string text = "{\"user_name\":\"DanKe\",\"user_age\":25}";

User user = JSON.ToData<User>(text);

Console.WriteLine(user.UserName);   // DanKe
Console.WriteLine(user.UserAge);    // 25
```

## 反序列化数组

```csharp
public class Order
{
    public int id { get; set; }
    public List<Item> items { get; set; }
}

public class Item
{
    public string name { get; set; }
    public double price { get; set; }
}

string text = """
    {
        "id": 1,
        "items": [
            { "name": "apple", "price": 1.5 },
            { "name": "banana", "price": 2.0 }
        ]
    }
    """;

Order order = JSON.ToData<Order>(text);
Console.WriteLine(order.items[0].name);   // apple
```

也可以直接反序列化到顶层 `List<T>`：

```csharp
List<int> nums = JSON.ToData<List<int>>("[1,2,3]");
List<Item> items = JSON.ToData<List<Item>>("[{\"name\":\"apple\",\"price\":1.5}]");
```

## 读取文件

如需从文件读取，请使用 `ToDataFromFile`：

```csharp
User user = JSON.ToDataFromFile<User>("user.json");
```

`ToData` / `ToData<T>` 只解析传入的文本，不再自动检测文件路径。

## 解析 JSON5

使用 `JSON5.ToData<T>` 解析 JSON5（注释、单引号字符串、无引号键名、多余逗号）：

```csharp
string json5 = """
    {
        name: 'DanKe',
        age: 25,
    }
    """;

User user = JSON5.ToData<User>(json5);
```

## 解析 JSONL

使用 `JSONL` 将 .jsonl 文件中的每一行反序列化为实体：

```csharp
List<User> users = JSONL.AllLineToData<User>("users.jsonl");

User second = JSONL.LineToData<User>("users.jsonl", 2);
```

> **限制**：当前版本不支持属性类型为自定义类的直接嵌套（非 `List<T>` 成员）。例如上面示例中的 `items` 必须声明为 `List<Item>` 才能正确反序列化。

[下一页：将 JsonData 编写为 JSON](./JsonData2Json.md)
