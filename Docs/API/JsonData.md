# JsonData

### *Class*

DanKeJson 的核心数据类，用一个统一的结构表示 JSON 值（对象、数组、数字、布尔、字符串、null）。

> 快速上手：[将 JSON 读取为 JsonData](../QuickStart/Json2JsonData.md)、[将 JsonData 编写为 JSON](../QuickStart/JsonData2Json.md)

## Constructors

| Name           | Summary |
| :------------- | :------ |
| [JsonData(Type)](#jsondatatype) | 创建指定类型的 JsonData（`Object` 会初始化 `map`，`Array` 会初始化 `array`，`None` 将 `json` 置为 `"null"`） |

### JsonData(Type)

```csharp
JsonData obj = new JsonData(JsonData.Type.Object);
JsonData arr = new JsonData(JsonData.Type.Array);
```

## Properties

| Name   | Value                      | Summary                                  |
| :----- | :------------------------- | :--------------------------------------- |
| `type` | [Type](./JsonData.Type.md) | 当前值的类型                             |
| `json` | string                     | 当前值的原始字符串表示（例如数字 `"42"`、字符串 `"\"text\""`） |

## Fields

| Name    | Type                         | Summary                              |
| :------ | :--------------------------- | :----------------------------------- |
| `map`   | `Dictionary<string, JsonData>` | 对象成员表，仅当 `type == Object` 时非空 |
| `array` | `List<JsonData>`             | 数组元素表，仅当 `type == Array` 时非空 |

## Indexers

| Name           | Value       | Summary |
| :------------- | :---------- | :------ |
| [this[string key]](#thisstring-key) | JsonData | 获取/设置对象的键值，键不存在时 `get` 返回 `null` |
| [this[int index]](#thisint-index)   | JsonData | 获取/设置数组元素，索引越界时 `get` 返回 `null`（`set` 忽略） |

### this[string key]

```csharp
JsonData obj = new JsonData(JsonData.Type.Object);
obj["name"] = "DanKe";          // 不存在则添加，存在则覆盖
```

### this[int index]

```csharp
JsonData arr = new JsonData(JsonData.Type.Array);
arr.Add("a");                   // 先添加元素才能用索引访问
JsonData first = arr[0];
```

## Methods

| Name            | Value | Summary                            |
| :-------------- | :---- | :--------------------------------- |
| [HasKey(string)](#haskeystring) | bool  | 判断对象是否包含指定键              |
| [Add(JsonData)](#addjsondata)   | void  | 向数组追加一个元素（隐式转换可传入任意基本类型） |

### HasKey(string)

```csharp
JsonData json = JSON.ToData("{\"a\":1,\"b\":2}");

bool hasA = json.HasKey("a");   // true
bool hasC = json.HasKey("c");   // false
```

### Add(JsonData)

```csharp
JsonData arr = new JsonData(JsonData.Type.Array);
arr.Add("hello");               // 隐式转换：string -> JsonData
arr.Add(42);                    // 隐式转换：int -> JsonData
arr.Add(true);
```

## Operators（隐式转换）

`JsonData` 与以下基本类型之间互相隐式转换，类型不匹配或解析失败时返回该类型的默认值（如 `0`、`false`、`null`）：

```csharp
JsonData json = JSON.ToData("{\"n\":42,\"s\":\"hi\",\"b\":true}");

int n = json["n"];        // 42
string s = json["s"];     // "hi"
bool b = json["b"];       // true

json["n"] = 7;            // 基本类型 -> JsonData
json["s"] = "world";
```

| Name                               | Value                     | Summary |
| :--------------------------------- | :------------------------ | :------ |
| implicit operator JsonData(string) | [JsonData](./JsonData.md) | 字符串包装为 JsonData（自动加引号） |
| implicit operator JsonData(bool)   | [JsonData](./JsonData.md) | 布尔包装为 JsonData   |
| implicit operator JsonData(int)    | [JsonData](./JsonData.md) | 数字包装为 JsonData   |
| implicit operator JsonData(long)   | [JsonData](./JsonData.md) | 数字包装为 JsonData   |
| implicit operator JsonData(float)  | [JsonData](./JsonData.md) | 数字包装为 JsonData（`NaN` / `±Infinity` 有特殊表示） |
| implicit operator JsonData(double) | [JsonData](./JsonData.md) | 数字包装为 JsonData（`NaN` / `±Infinity` 有特殊表示） |
| implicit operator JsonData(sbyte)  | [JsonData](./JsonData.md) | 数字包装为 JsonData   |
| implicit operator JsonData(short)  | [JsonData](./JsonData.md) | 数字包装为 JsonData   |
| implicit operator JsonData(uint)   | [JsonData](./JsonData.md) | 数字包装为 JsonData   |
| implicit operator JsonData(ulong)  | [JsonData](./JsonData.md) | 数字包装为 JsonData   |
| implicit operator JsonData(ushort) | [JsonData](./JsonData.md) | 数字包装为 JsonData   |
| implicit operator string(JsonData) | string                    | 解包为 string（非 String 类型返回 `null`） |
| implicit operator bool(JsonData)   | bool                      | 解包为 bool          |
| implicit operator int(JsonData)    | int                       | 解包为 int           |
| implicit operator long(JsonData)   | long                      | 解包为 long          |
| implicit operator float(JsonData)  | float                     | 解包为 float         |
| implicit operator double(JsonData) | double                    | 解包为 double        |
| implicit operator sbyte(JsonData)  | sbyte                     | 解包为 sbyte         |
| implicit operator short(JsonData)  | short                     | 解包为 short         |
| implicit operator uint(JsonData)   | uint                      | 解包为 uint          |
| implicit operator ulong(JsonData)  | ulong                     | 解包为 ulong         |
| implicit operator ushort(JsonData) | ushort                    | 解包为 ushort        |

> 注意：当 `jsonData == null`（例如索引不存在的键）或类型不匹配时，解包操作返回默认值而**不会**抛出异常。
