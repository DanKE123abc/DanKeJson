# 更新日志 / Changelog

[中文](#中文) | [English](#english)

本文件记录 DanKeJson 的版本变更，版本号遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

---

## 中文

## [1.6.0] - 2026-10-04

本次发布以修复缺陷与补齐互操作能力为主，共修复 30 余处问题，并新增 236 个单元测试（`DanKeJson.Tests`）。

### 新增

**解析**

- `JsonSettings.MaxDepth`（默认 256）与 `JsonDepthLimitException`：深层嵌套与自引用结构不再触发不可捕获的 `StackOverflowException`（解析、对象图序列化与反序列化同样受保护）
- 忽略文本开头的 UTF-8 BOM
- 键名支持 `\"`、`\\`、`\uXXXX` 等转义
- 数字字面量规范化：`01` → `1`、`007` → `7`、`00.5` → `0.5`，保证再序列化时仍是合法 JSON

**反序列化**

- 支持 `Dictionary<string,T>` 成员、顶层 `Dictionary<string,T>` 与 `List<Dictionary<...>>` 元素
- 支持可空值类型成员（`int?` / `bool?` / `Color?` 等），JSON `null` 保持 null
- 支持 `byte` 与 `char` 成员
- 枚举成员支持字符串名称（大小写不敏感，支持 Flags 的逗号写法与数字字符串）
- `object` 成员按 JSON 实际类型映射：`string` / `long` / `double` / `bool` / `List<object>` / `Dictionary<string,object>` / `null`
- 集合接口成员（`IEnumerable<T>`、`IList<T>`、`ICollection<T>`、`IReadOnlyList<T>`、`IReadOnlyCollection<T>`、`IDictionary<K,V>`、`IReadOnlyDictionary<K,V>`）自动映射到 `List<T>` / `Dictionary<K,V>`
- `JsonData` 目标（成员、`List<JsonData>` 元素、顶层）直接保留原始节点
- 抽象类、接口与没有公共无参构造的成员类型改为跳过该成员，不再抛出 `MissingMethodException`

**序列化**

- 非 `IList` 的可枚举类型（`Stack<T>`、`Queue<T>`、`HashSet<T>`、LINQ 结果等）按数组输出
- `Type` / `MemberInfo` / `Assembly` / `Module` 输出为文本
- `char` 输出为字符串（如 `"a"`）

**JSON5**

- 支持 `0x` / `0X` 十六进制数字（可带正负号）
- 支持 `\x`、`\v`、`\0`、`\'` 转义与字符串续行
- 支持省略整数或小数部分的小数：`.5`、`5.`、`.5e1`、`5.e3`
- 无引号键支持 `$` 与 `\uXXXX` 标识符转义
- 注释清理改为字符串感知的单遍扫描：不再误删字符串内部的 `/*...*/`，且性能更好

**JSONL**

- 写文件改为 UTF-8 无 BOM、行分隔符统一为 `\n`（与返回值一致）
- 列表中的 `null` 元素写成 JSON 的 `null`
- 读取时跳过无法解析的行，不再插入 `null` 元素

**其它**

- 新增 `DanKeJson.Tests` 测试项目（xunit，236 个用例）并加入解决方案

### 修复

- 顶层枚举序列化输出成员名（`Blue`）导致非法 JSON
- 公共索引器导致序列化与反序列化抛出 `TargetParameterCountException`
- `List<JsonData>`、`ToData<List<JsonData>>` 抛出 `MissingMethodException`
- 顶层字典与 `List<Dictionary<...>>` 元素静默得到空字典
- `byte` / `char` / 可空值类型成员被静默忽略
- `JsonData.HasKey(null)`、`data[null]` 抛出 `ArgumentNullException`
- 自定义字典的 `null` 键导致序列化抛异常（改为写成空键）
- JSON5 字符串内部的 `/*...*/` 被当作注释删除
- 孤立代理项序列化成非法 JSON（现转义为 `\uXXXX`，合法代理对原样保留）
- 键名中的转义引号导致整段解析失败
- 前导 BOM 导致解析失败
- 抽象类 / 接口 / 无公共无参构造的成员类型抛出 `MissingMethodException`
- 属性 getter / setter 抛出的异常被包装成 `TargetInvocationException`（现还原原始异常与堆栈）
- `JsonData` 的 `(string)` 转换对畸形节点抛出 `ArgumentOutOfRangeException`
- `JSON.ToJson(typeof(T))` 抛出 `TargetInvocationException`
- `JsonData` 数值隐式转换使用当前区域性（de-DE 下 `1.5` 被解析为 `15`）
- `NaN` / `Infinity` 序列化成非法 JSON
- JSONL 写文件遇到 `null` 元素抛出 `NullReferenceException`
- 数字字面量保留前导零，再序列化产生非法 JSON

### 行为变更（升级注意）

- **顶层枚举**序列化结果由成员名（`Blue`）变为基础类型数字（`2`）
- **`char`** 序列化结果由字符编码（`97`）变为字符串（`"a"`）
- **`NaN` / `Infinity`** 在纯 JSON 中输出 `null`（与 `JSON.stringify` 一致）；JSON5 仍保留 `NaN` / `Infinity` 字面量
- **嵌套深度**超过 `JsonSettings.MaxDepth`（默认 256）时抛出 `JsonDepthLimitException`；需要更深的文档请调整该设置（设为 `<= 0` 表示不限制，但会重新暴露崩溃风险）
- **数字字面量**会被规范化：`JSON.ToData("01").json` 由 `"01"` 变为 `"1"`
- **JSONL 文件**不再带 BOM，行分隔符统一为 `\n`
- **可空成员**遇到 JSON `null` 时保持 `null`，不再写入值类型默认值
- **反射对象**（`Type` 等）输出为文本，不再抛异常
- **JSONL 写文件**要求目标目录已存在，否则抛出 `DirectoryNotFoundException`（.NET 惯例，已写入 API 文档）
- 成员名仍是**精确匹配**且**不做类型强转**（如 `{"i":"5"}` 不会写入 `int` 成员），该语义已写入文档并加测试锁定

### 平台

net10.0 / net9.0 / net8.0 / net7.0 / net6.0 / net5.0 / netstandard2.1 / netcoreapp3.0 / netcoreapp3.1

## [1.5.1] - 2026-08-10

- 支持使用公共字段定义实体类（LitJSON 风格，无需 `{ get; set; }`）
- 反序列化支持更多类型：数组、`decimal`、`DateTime` / `DateTimeOffset` / `TimeSpan` / `Guid`、枚举、`List<T>`、`IDictionary`
- 序列化支持列表 / 数组 / 字典 / 枚举，并加入循环引用检测
- JSONL 写入增加空路径保护

> 1.5.1 之前的版本变更未整理。

---

## English

## [1.6.0] - 2026-10-04

This release focuses on bug fixes and interoperability, fixing more than 30 issues and adding a test project (`DanKeJson.Tests`) with 236 test cases.

### Added

- `JsonSettings.MaxDepth` (default 256) and `JsonDepthLimitException`: deep nesting and self-referencing structures now throw a catchable exception instead of terminating the process with an uncatchable `StackOverflowException` (parsing, object graph serialization and deserialization)
- Leading UTF-8 BOM is ignored; escaped characters (`\"`, `\\`, `\uXXXX`) are supported in key names
- Number literals are normalized (`01` → `1`, `007` → `7`) so re-serialization always produces valid JSON
- `Dictionary<string,T>` members, top-level dictionaries and `List<Dictionary<...>>` elements
- Nullable value type members (`int?`, `bool?`, ...); JSON `null` keeps them `null`
- `byte` and `char` members; enum members accept names (case-insensitive, flags syntax) as well as numbers
- `object` members are mapped by the actual JSON type (`string` / `long` / `double` / `bool` / `List<object>` / `Dictionary<string,object>`)
- Collection interface members are mapped to concrete `List<T>` / `Dictionary<K,V>` implementations
- `JsonData` targets keep the original node; uninstantiable member types are skipped instead of throwing
- Non-`IList` enumerables (`Stack<T>`, `Queue<T>`, `HashSet<T>`, LINQ results) serialize as arrays; reflection objects (`Type`, `MemberInfo`, `Assembly`, `Module`) serialize as text
- JSON5: hexadecimal numbers, `\x` `\v` `\0` `\'` escapes, string continuation, `.5` / `5.` decimals, `$` and `\uXXXX` in unquoted keys, string-aware comment removal
- JSONL: files are written as UTF-8 without BOM using `\n`; `null` elements are written as `null`; unparsable lines are skipped

### Fixed

More than 30 defects, including: top-level enums emitting member names (invalid JSON), public indexers throwing `TargetParameterCountException`, `MissingMethodException` for `List<JsonData>` and uninstantiable member types, silently empty dictionaries, silently ignored `byte` / `char` / nullable members, `ArgumentNullException` from `HasKey(null)` and `data[null]`, JSON5 comments deleting text inside strings, unpaired surrogates producing invalid JSON, escaped quotes in key names failing the whole document, leading BOM failing to parse, `TargetInvocationException` wrapping getter/setter exceptions, `JSON.ToJson(typeof(T))` throwing, culture-sensitive `JsonData` numeric casts, `NaN` / `Infinity` producing invalid JSON, and `NullReferenceException` when writing JSONL with `null` elements.

### Changed

- Top-level enums now serialize to numbers (`2`) instead of member names (`Blue`)
- `char` now serializes to a string (`"a"`) instead of its code point (`97`)
- `NaN` / `Infinity` serialize to `null` in plain JSON (matching `JSON.stringify`); JSON5 keeps the literals
- Nesting deeper than `JsonSettings.MaxDepth` (default 256) now throws `JsonDepthLimitException`
- Number literals are normalized, JSONL files no longer carry a BOM and use `\n`, nullable members stay `null` on JSON `null`
- Member matching is exact and no implicit type coercion is performed (`{"i":"5"}` will not populate an `int` member)

### Platforms

net10.0 / net9.0 / net8.0 / net7.0 / net6.0 / net5.0 / netstandard2.1 / netcoreapp3.0 / netcoreapp3.1

## [1.5.1] - 2026-08-10

- Support for entity classes defined with public fields (LitJSON style, no `{ get; set; }` required)
- Deserialization support for arrays, `decimal`, `DateTime` / `DateTimeOffset` / `TimeSpan` / `Guid`, enums, `List<T>` and `IDictionary`
- Serialization support for lists, arrays, dictionaries and enums, plus circular reference detection
- JSONL write guard for empty file paths

> Changes before 1.5.1 are not documented.
