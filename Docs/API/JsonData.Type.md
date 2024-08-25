# JsonData.Type

### *enum*

## code

```C#
public enum Type
{
	Object,
	Array,
	Number,
	Boolean,
	String,
    None
}
```

### Type

| JsonData.Type | System.Type                                         |
| :------------ | :-------------------------------------------------- |
| Object        | class                                               |
| Array         | List<>                                              |
| Number        | double,float,int,long,sbyte,short,uint,ulong,ushort |
| Boolean       | bool                                                |
| String        | string                                              |
| None          | null                                                |

