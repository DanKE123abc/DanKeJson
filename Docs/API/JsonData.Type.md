# JsonData.Type

### *enum*

`JsonData.Type` 用于描述 [JsonData](./JsonData.md) 当前值的类型。

## code

```C#
public enum Type
{
	Object,   // class
	Array,    // list
	Number,   // int...and so on
	Boolean,  // bool
	String,   // string
	None      // null
}
```

## Type

| JsonData.Type | System.Type                                         |
| :------------ | :-------------------------------------------------- |
| Object        | class                                               |
| Array         | List<>                                              |
| Number        | double, float, int, long, sbyte, short, uint, ulong, ushort |
| Boolean       | bool                                                |
| String        | string                                              |
| None          | null                                                |

> 提示：`None` 表示 `null`。通过索引器访问不存在的键返回 `null`（而非 `None`），未定义的值在解析时会被转换为 `None`。
