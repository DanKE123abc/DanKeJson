# JSON

### *Class*

用于标准 JSON 的序列化与反序列化（`static` 类，无需实例化）。

> 快速上手：[将 JSON 读取为 JsonData](../QuickStart/Json2JsonData.md)、[将 JSON 读取为 .NET 对象](../QuickStart/Json2Object.md)、[将 JsonData 编写为 JSON](../QuickStart/JsonData2Json.md)、[将 .NET 对象编写为 JSON](../QuickStart/Object2Json.md)

## Methods

| Name                            | Value                     | Summary |
| :------------------------------ | :------------------------ | :------ |
| [ToData(string)](#todatastring)                              | [JsonData](./JsonData.md) | 将 JSON 文本解析为 JsonData |
| [ToData\<T\>(string)](#todattstring)                         | T                         | 将 JSON 文本反序列化为实体类 T |
| [ToDataFromFile(string)](#todatafromfilestring)              | [JsonData](./JsonData.md) | 读取 JSON 文件并解析为 JsonData |
| [ToDataFromFile\<T\>(string)](#todatafromfilestring)         | T                         | 读取 JSON 文件并反序列化为实体类 T |
| [ToJson(JsonData)](#tojsonjsondata)                          | string                    | 将 JsonData 序列化为 JSON 字符串 |
| [ToJson(object)](#tojsonobject)                              | string                    | 将 .NET 对象序列化为 JSON 字符串 |

---

### ToData(string)

`public static JsonData ToData(string text)`

将 JSON 文本解析为 [JsonData](./JsonData.md)。

- 只解析传入的文本，不再自动检测文件路径；读取文件请使用 [ToDataFromFile](#todatafromfilestring)。
- 文本无法被完整解析时返回 `null`。

```csharp
JsonData json = JSON.ToData("{\"name\":\"DanKe\",\"age\":25}");
```

### ToData\<T\>(string)

`public static T ToData<T>(string text) where T : class, new()`

将 JSON 文本反序列化为实体类 `T`（`T` 需为带无参构造函数的 class）。

- 支持 `string` / `bool` / 数字类型 / `List<T>` / `JsonData` 属性，详见 [将 JSON 读取为 .NET 对象](../QuickStart/Json2Object.md)。

```csharp
User user = JSON.ToData<User>("{\"name\":\"DanKe\",\"age\":25}");
```

### ToDataFromFile(string)

`public static JsonData ToDataFromFile(string filePath)`

读取指定路径的 JSON 文件，并将内容解析为 [JsonData](./JsonData.md)。

```csharp
JsonData json = JSON.ToDataFromFile("config.json");
```

### ToDataFromFile\<T\>(string)

`public static T ToDataFromFile<T>(string filePath) where T : class, new()`

读取指定路径的 JSON 文件，并将内容反序列化为实体类 `T`。

```csharp
User user = JSON.ToDataFromFile<User>("user.json");
```

### ToJson(JsonData)

`public static string ToJson(JsonData json)`

将 [JsonData](./JsonData.md) 序列化为标准 JSON 字符串，传入 `null` 时返回 `null`。

```csharp
JsonData json = JSON.ToData("{\"a\":1}");
string text = JSON.ToJson(json);   // {"a":1}
```

### ToJson(object)

`public static string ToJson(object jsonObject)`

将 .NET 对象（实体类实例）序列化为标准 JSON 字符串，传入 `null` 时返回 `null`。

- 序列化所有可读（public getter）属性，键名为属性原名。
- `[JsonProperty]` 特性不影响序列化。

```csharp
User user = new User { name = "DanKe", age = 25 };
string text = JSON.ToJson(user);   // {"name":"DanKe","age":25}
```

> 参见：[JSONL](./JSONL.md)（.jsonl 文件的读写）、[JSON5](./JSON5.md)（JSON5 格式）
