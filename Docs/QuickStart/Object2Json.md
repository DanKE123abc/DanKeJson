# 将 .NET 对象编写为 JSON（序列化）

[上一页：将 JsonData 编写为 JSON](./JsonData2Json.md)

## 快速入门

使用 `JSON.ToJson(object)` 将实体类实例序列化为 JSON 字符串：

```csharp
using DanKeJson;

public class User
{
    public string name { get; set; }
    public int age { get; set; }
    public bool isVip { get; set; }
}

User user = new User
{
    name = "DanKe",
    age = 25,
    isVip = true
};

string text = JSON.ToJson(user);
Console.WriteLine(text);   // {"name":"DanKe","age":25,"isVip":true}
```

传入 `null` 时返回 `null`。

## 序列化规则

- 序列化对象所有**可读（public getter）**的属性。
- 支持 `string`、`bool`、数字类型（`int` / `long` / `float` / `double` / `sbyte` / `short` / `uint` / `ulong` / `ushort`）。
- 支持 `List<T>`，以及嵌套的自定义类。
- `null` 属性序列化为 `null`。
- 键名使用属性的原始名称，`[JsonProperty]` 特性只影响[反序列化](./Json2Object.md)，不影响序列化。

## 嵌套对象与数组

```csharp
using DanKeJson;

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

Order order = new Order
{
    id = 1,
    items = new List<Item>
    {
        new Item { name = "apple", price = 1.5 },
        new Item { name = "banana", price = 2.0 }
    }
};

string text = JSON.ToJson(order);
Console.WriteLine(text);
// {"id":1,"items":[{"name":"apple","price":1.5},{"name":"banana","price":2.0}]}
```

## 序列化为 JSON5

使用 `JSON5.ToJson(object)` 序列化，并可通过 `Json5Options` 定制格式：

```csharp
using DanKeJson;

User user = new User { name = "DanKe", age = 25 };

var options = new Json5Options
{
    KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes,
    StringQuoteStyle = Json5Options.StringQuoteType.SingleQuote,
    AddTailingCommaForObject = true
};

string json5 = JSON5.ToJson(user, options);
Console.WriteLine(json5);   // {name:'DanKe',age:25,}
```

`Json5Options` 的完整说明见 [将 JsonData 编写为 JSON](./JsonData2Json.md)。

## 序列化文件

结合 [JSON.ToData](./Json2JsonData.md) 的文件读取能力，可以轻松实现「读文件 → 修改 → 写回」：

```csharp
// 读取并解析
User user = JSON.ToData<User>("user.json");

// 修改
user.age = 26;

// 序列化并写回
File.WriteAllText("user.json", JSON.ToJson(user));
```
