# JSONL

### *Class*

用于 [JSON Lines](https://jsonlines.org/)（.jsonl）文件的读写。JSONL 文件中每行是一条独立的 JSON 数据。

> 快速上手：[将 JSON 读取为 JsonData](../QuickStart/Json2JsonData.md)、[将 JSON 读取为 .NET 对象](../QuickStart/Json2Object.md)

## Methods

| Name                                | Value                     | Summary |
| :---------------------------------- | :------------------------ | :------ |
| [AllLineToData(string)](#alllinetodatastring)          | `List<JsonData>`          | 读取文件中每一行并解析为 JsonData 列表（自动跳过空行） |
| [AllLineToData\<T\>(string)](#alllinetodatattring)     | `List<T>`                 | 读取文件中每一行并反序列化为实体类列表（自动跳过空行） |
| [LineToData(string, int)](#linetodatastring-int)       | [JsonData](./JsonData.md) | 读取指定行（行号从 1 开始）并解析为 JsonData，行不存在时返回 `null` |
| [LineToData\<T\>(string, int)](#linetodatattring-int)  | T                         | 读取指定行并反序列化为实体类，行不存在时返回 `null` |
| [ListToJson(List\<JsonData\>)](#listtojsonlistjsondata) | string                    | 将 JsonData 列表序列化为 JSON Lines 文本（每行一条） |
| [ListToJson\<T\>(List\<T\>)](#listtojsontlistt)         | string                    | 将实体类列表序列化为 JSON Lines 文本（每行一条） |

---

### AllLineToData(string)

`public static List<JsonData> AllLineToData(string filePath)`

逐行读取 .jsonl 文件，将每一行解析为 [JsonData](./JsonData.md) 并加入列表，空行会被自动跳过。

```csharp
List<JsonData> lines = JSONL.AllLineToData("data.jsonl");
```

### AllLineToData\<T\>(string)

`public static List<T> AllLineToData<T>(string filePath) where T : class, new()`

逐行读取 .jsonl 文件，将每一行反序列化为实体类 `T` 并加入列表，空行会被自动跳过。

```csharp
List<User> users = JSONL.AllLineToData<User>("users.jsonl");
```

### LineToData(string, int)

`public static JsonData LineToData(string filePath, int lineNumber)`

读取指定行并解析为 [JsonData](./JsonData.md)。`lineNumber` 从 1 开始计数，行不存在或文件无法读取时返回 `null`。

```csharp
JsonData second = JSONL.LineToData("data.jsonl", 2);
```

### LineToData\<T\>(string, int)

`public static T LineToData<T>(string filePath, int lineNumber) where T : class, new()`

读取指定行并反序列化为实体类 `T`，规则与 `LineToData` 一致。

```csharp
User second = JSONL.LineToData<User>("users.jsonl", 2);
```

### ListToJson(List\<JsonData\>)

`public static string ListToJson(List<JsonData> jsonDataList)`

将 [JsonData](./JsonData.md) 列表序列化为 JSON Lines 文本，每条 JsonData 占一行，传入 `null` 时返回 `null`。

```csharp
List<JsonData> lines = JSONL.AllLineToData("data.jsonl");
string jsonl = JSONL.ListToJson(lines);
```

### ListToJson\<T\>(List\<T\>)

`public static string ListToJson<T>(List<T> jsonDataList) where T : class, new()`

将实体类列表序列化为 JSON Lines 文本，每个元素占一行，传入 `null` 时返回 `null`。

```csharp
List<User> users = JSONL.AllLineToData<User>("users.jsonl");
string jsonl = JSONL.ListToJson(users);
```
