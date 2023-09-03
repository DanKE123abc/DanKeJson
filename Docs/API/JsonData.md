# JsonData

### *Class*



## Constructors

| Name           | Summary |
| :------------- | :------ |
| JsonData(Type) |         |



## Properties

| Name | Value                      | Summary |
| :--- | :------------------------- | :------ |
| json | string                     |         |
| type | [Type](.\JsonData.Type.md) |         |



## Methods

| Name           | Value       | Summary             |
| :------------- | :---------- | :------------------ |
| HasKey(string) | bool        | 获取Object是否有Key |
| Add(JsonData)  | void        | 往Array里添加Value  |
| Add(object)    | void        | 往Array里添加Value  |
| Equals(object) | bool        |                     |
| ToString()     | string      |                     |
| GetType()      | System.Type |                     |
| GetHashCode()  | int         |                     |



## Operators

| Name                               | Value                     | Summary |
| :--------------------------------- | :------------------------ | :------ |
| implicit operator JsonData(string) | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(bool)   | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(int)    | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(long)   | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(float)  | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(double) | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(sbyte)  | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(short)  | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(uint)   | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(ulong)  | [JsonData](./JsonData.md) |         |
| implicit operator JsonData(ushort) | [JsonData](./JsonData.md) |         |
| implicit operator string(JsonData) | string                    |         |
| implicit operator bool(JsonData)   | bool                      |         |
| implicit operator int(JsonData)    | int                       |         |
| implicit operator long(JsonData)   | long                      |         |
| implicit operator float(JsonData)  | float                     |         |
| implicit operator double(JsonData) | double                    |         |
| implicit operator sbyte(JsonData)  | sbyte                     |         |
| implicit operator short(JsonData)  | short                     |         |
| implicit operator uint(JsonData)   | uint                      |         |
| implicit operator ulong(JsonData)  | ulong                     |         |
| implicit operator ushort(JsonData) | ushort                    |         |