# JSON5

### *Class*

用于 JSON5（JSON for Humans）的序列化与反序列化（`static` 类，无需实例化）。

与标准 [JSON](./JSON.md) 相比，JSON5 额外支持：

- 单行注释 `//` 与多行注释 `/* */`（通过正则表达式移除，可能影响解析性能）
- 字符串使用单引号 `'...'`
- 键名无需引号
- 数组、对象允许多余逗号

> 快速上手：[将 JSON 读取为 JsonData](../QuickStart/Json2JsonData.md)、[将 JsonData 编写为 JSON](../QuickStart/JsonData2Json.md)

## Methods

| Name                            | Value                     | Summary |
| :------------------------------ | :------------------------ | :------ |
| [ToData(string)](#todatastring)                              | [JsonData](./JsonData.md) | 将 JSON5 文本解析为 JsonData |
| [ToData\<T\>(string)](#todattstring)                         | T                         | 将 JSON5 文本反序列化为实体类 T |
| [ToDataFromFile(string)](#todatafromfilestring)              | [JsonData](./JsonData.md) | 读取 JSON5 文件并解析为 JsonData |
| [ToDataFromFile\<T\>(string)](#todatafromfilestring)         | T                         | 读取 JSON5 文件并反序列化为实体类 T |
| [ToJson(JsonData, options=null)](#tojsonjsondata-optionsnull)    | string                    | 将 JsonData 序列化为 JSON5 字符串 |
| [ToJson(object, options=null)](#tojsonobject-optionsnull)        | string                    | 将 .NET 对象序列化为 JSON5 字符串 |

## Json5Options

序列化时的格式选项，不传或传 `null` 时使用默认值（输出与标准 JSON 一致）。

| 属性                       | 类型                           | 默认值          | 说明                        |
| :------------------------- | :----------------------------- | :-------------- | :-------------------------- |
| `KeyNameStyle`             | [KeyNameType](#keynametype)    | `WithQuotes`    | 键名是否使用引号            |
| `StringQuoteStyle`         | [StringQuoteType](#stringquotetype) | `DoubleQuote`   | 字符串使用单引号还是双引号  |
| `AddTailingCommaForObject` | `bool`                         | `false`         | 对象是否添加末尾逗号 `,`    |
| `AddTailingCommaForArray`  | `bool`                         | `false`         | 数组是否添加末尾逗号 `,`    |

### KeyNameType

```csharp
public enum KeyNameType
{
    WithQuotes,     // "key": value
    WithoutQuotes   // key: value
}
```

### StringQuoteType

```csharp
public enum StringQuoteType
{
    SingleQuote,    // 'text'
    DoubleQuote     // "text"
}
```

---

### ToData(string)

`public static JsonData ToData(string text)`

将 JSON5 文本解析为 [JsonData](./JsonData.md)。

- 只解析传入的文本，不再自动检测文件路径；读取文件请使用 [ToDataFromFile](#todatafromfilestring)。
- 解析前会先通过 `CommentParser` 移除注释。

```csharp
JsonData json = JSON5.ToData("{ name: 'DanKe', age: 25, }");   // 允许无引号键名、单引号、末尾逗号
```

### ToData\<T\>(string)

`public static T ToData<T>(string text) where T : class, new()`

将 JSON5 文本反序列化为实体类 `T`，规则与 [JSON.ToData\<T\>](./JSON.md#todattstring) 一致。

```csharp
User user = JSON5.ToData<User>("{ name: 'DanKe', age: 25 }");
```

### ToDataFromFile(string)

`public static JsonData ToDataFromFile(string filePath)`

读取指定路径的 JSON5 文件，并将内容解析为 [JsonData](./JsonData.md)。

```csharp
JsonData json = JSON5.ToDataFromFile("config.json5");
```

### ToDataFromFile\<T\>(string)

`public static T ToDataFromFile<T>(string filePath) where T : class, new()`

读取指定路径的 JSON5 文件，并将内容反序列化为实体类 `T`。

```csharp
User user = JSON5.ToDataFromFile<User>("user.json5");
```

### ToJson(JsonData, options=null)

`public static string ToJson(JsonData json, Json5Options options = null)`

将 [JsonData](./JsonData.md) 序列化为 JSON5 字符串，传入 `null` 时返回 `null`。

```csharp
JsonData json = JSON.ToData("{\"name\":\"DanKe\"}");
string json5 = JSON5.ToJson(json);   // {"name":"DanKe"}

var options = new Json5Options
{
    KeyNameStyle = Json5Options.KeyNameType.WithoutQuotes,
    StringQuoteStyle = Json5Options.StringQuoteType.SingleQuote,
    AddTailingCommaForObject = true
};
string json5 = JSON5.ToJson(json, options);   // {name:'DanKe',}
```

### ToJson(object, options=null)

`public static string ToJson(object jsonObject, Json5Options options = null)`

将 .NET 对象序列化为 JSON5 字符串，序列化规则与 [JSON.ToJson(object)](./JSON.md#tojsonobject) 一致，传入 `null` 时返回 `null`。

```csharp
User user = new User { name = "DanKe", age = 25 };
string json5 = JSON5.ToJson(user, options);
```
