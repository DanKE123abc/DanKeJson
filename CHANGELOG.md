# 更新日志 / Changelog

[中文](#中文) | [English](#english)

本文件记录 DanKeJson 的版本变更，版本号遵循 [语义化版本](https://semver.org/lang/zh-CN/)。

---

## 中文

## [1.6.0] - 2026-10-04

本次发布以修复缺陷与补齐互操作能力为主：**新增 35 项能力与改进，修复 37 处缺陷**，并新增 236 个单元测试（`DanKeJson.Tests`）。

### 新增

#### 解析

1. **前导 UTF-8 BOM 忽略**：`JSON.ToData` / `ToData<T>`、`JSON5.ToData` / `ToData<T>` 四个入口都会剥离 BOM。
2. **键名转义**：`\"`、`\\`、`\/`、`\b`、`\f`、`\n`、`\r`、`\t`、`\uXXXX`（JSON 与 JSON5 的带引号键都支持，重复键判断也按解码后的键名）。
3. **数字字面量规范化**：`01` → `1`、`007` → `7`、`-01` → `-1`、`00.5` → `0.5`（`0`、`-0`、`0.5`、`1e3` 保持原样）。
4. **`JsonSettings.MaxDepth`**（默认 256，`<= 0` 表示不限制）与 **`JsonSettings.DefaultMaxDepth`** 常量。
5. **`JsonDepthLimitException`**：新增公共异常类型，带 `MaxDepth` 属性与清晰消息。
6. **深度保护覆盖范围**：JSON 解析、JSON5 解析、对象图序列化、对象图反序列化、`object` 成员递归、自引用集合与自引用对象。

#### 反序列化

7. **`Dictionary<string,T>` 成员**：值支持基本类型、嵌套对象、列表、嵌套字典；键支持 `string` / `int` / `enum` / `Guid`。
8. **顶层 `Dictionary<string,T>`** 与 **`List<Dictionary<...>>` 元素**（与成员路径统一实现）。
9. **可空值类型成员**：`int?`、`bool?`、`Color?`、`DateTime?`、`Guid?` 等；JSON `null` 时保持 `null`（不再写入默认值）。
10. **`byte` 成员**。
11. **`char` 成员**：支持 JSON 字符串（取首字符）与数字码点（`97` → `'a'`）。
12. **枚举成员支持字符串名称**：大小写不敏感、Flags 逗号写法、数字字符串（`"1"`）。
13. **`object` 成员按 JSON 实际类型动态映射**：`string` / `long` / `double` / `bool` / `List<object>` / `Dictionary<string,object>` / `null`。
14. **`List<object>` 元素与 `Dictionary<*,object>` 值**同样走动态映射。
15. **集合接口成员自动映射到具体实现**：`IEnumerable<T>`、`IList<T>`、`ICollection<T>`、`IReadOnlyList<T>`、`IReadOnlyCollection<T>` → `List<T>`；`IDictionary<K,V>`、`IReadOnlyDictionary<K,V>` → `Dictionary<K,V>`。
16. **`JsonData` 目标保留原始节点**：普通成员、`List<JsonData>` 元素、顶层 `List<JsonData>`、字典值。
17. **无法实例化的成员类型改为跳过**（抽象类、接口、无公共无参构造，如 `Uri`、抽象基类），其余成员照常填充。

#### 序列化

18. **非 `IList` 的可枚举类型按数组输出**：`Stack<T>`、`Queue<T>`、`HashSet<T>`、LINQ 结果等（按枚举顺序）。
19. **反射对象按文本输出**：`Type`、`MemberInfo`（`MethodInfo` / `PropertyInfo` / …）、`Assembly`、`Module`。
20. **`char` 序列化为字符串**（`'a'` → `"a"`），并新增 `JsonData` 的 `implicit operator JsonData(char)`，避免 char 经由 `int` 变成字符编码。
21. **循环引用检测覆盖自引用集合**（配合深度保护兜底）。

#### JSON5

22. **十六进制数字**：`0x10` / `0X10` / `-0x10` / `+0xff`（输出十进制）。
23. **转义补齐**：`\x` 十六进制转义、`\v`、`\0`（后跟数字时按原样保留）、`\'`。
24. **字符串续行**：`\` + LF / CRLF / CR / U+2028 / U+2029 会被移除。
25. **小数写法**：`.5` → `0.5`、`5.` → `5`、`-.5` → `-0.5`、`.5e1` → `0.5e1`、`5.e3` → `5e3`。
26. **无引号键支持 `$`**（解析与序列化两侧一致）。
27. **无引号键支持 `\uXXXX` 标识符转义**（`{\u0061bc:1}` → 键 `abc`）。
28. **注释清理改为字符串感知的单遍扫描**：字符串内部的 `//`、`/*...*/` 不再被删；顺带把 20 万字符文本的清理耗时从 25 ms 降到 3 ms。

#### JSONL

29. **写文件规范**：UTF-8 **无 BOM**、行分隔符统一 `\n`（与返回值一致）。
30. **null 元素写为 JSON 的 `null`**（读回是 `Type.None` 节点）。
31. **读取时跳过无法解析的行**（以前会插入 `null` 元素）。

#### 测试与文档

32. **新增 `DanKeJson.Tests`**：xunit 2.9.3 + Microsoft.NET.Test.Sdk 17.14.1 + coverlet.collector 6.0.4，目标 `net10.0`，**236 个用例**，并加入 `DanKeJson.sln`。
33. **测试辅助**：`TempWorkspace`（临时目录隔离）、`AssemblyInfo.cs`（关闭并行，因测试会修改 `JsonSettings.MaxDepth` 与区域性）。
34. **API 文档补充**：`JSON.ToData<T>`（成员名精确匹配、不做类型强转）、`JSON.ToJson`（NaN / Infinity → null）、`JSONL.ListToJson`（无 BOM、`\n`、null 元素、目录必须存在）。
35. **新增 `CHANGELOG.md`** 与 README 的更新日志入口；README / README_en 的版本号与安装命令更新为 1.6.0，并勾选已完成的日期格式支持。

### 修复

#### 成员类型与值

1. **可空值类型成员从不被赋值** → 按 `Nullable` 基础类型转换，JSON `null` 保持 `null`。
2. **`byte` / `char` 成员不被赋值** → 补上对应分支。
3. **`Dictionary<string,T>` 成员反序列化得到空字典**（静默丢数据）→ 正常填充。
4. **顶层枚举序列化输出成员名**（`Blue`，非法 JSON）→ 输出基础类型数字（`2`）。
5. **枚举成员不支持字符串**（`"Blue"` 静默变成默认值 0）→ 支持名称解析，未知名称保持默认值。
6. **键名中的转义引号导致整段解析失败** → 正确解析。
7. **前导 BOM 导致解析失败** → 忽略 BOM。
8. **JSON5 块注释清理误删字符串内容**（`"a/*b*/c"` → `"ac"`）→ 原样保留。
9. **JSON5 不支持 `0x` 十六进制** → 支持。
10. **`JsonData` 的 `double` / `float` 隐式转换使用当前区域性**（de-DE 下 `"1.5"` 得到 15）→ 全部数值转换改用 `InvariantCulture` 与明确的 `NumberStyles`。

#### 崩溃与静默错误

11. **深层嵌套触发 `StackOverflowException`（不可捕获、直接终止进程）**：JSON 约 3500 层、JSON5 约 3000 层、对象图序列化约 900 层、反序列化约 1200 层 → 统一抛 `JsonDepthLimitException`。
12. **序列化带公共索引器的对象抛 `TargetParameterCountException`**（.NET 10 的 LINQ 迭代器也命中）。
13. **反序列化时 JSON 里出现 `Item` 键**（目标类有索引器）**抛 `TargetParameterCountException`**。
14. **非 `IList` 集合被当普通对象序列化**：`Stack<int>` → `{"Count":3,"Capacity":3}`、`HashSet` → `{"Comparer":{}}` 等静默错误结果。
15. **`List<JsonData>` / `JSON.ToData<List<JsonData>>` 抛 `MissingMethodException`**（`JsonData` 没有无参构造）。
16. **顶层 `Dictionary<string,T>` 与 `List<Dictionary<...>>` 元素静默得到空字典**。
17. **自定义字典返回 `null` 键时序列化抛 `ArgumentNullException`** → 写为空键（防御性处理）。
18. **`JsonData.HasKey(null)`、`data[null]`（读 / 写）抛 `ArgumentNullException`** → 返回 false / null、写入忽略。
19. **JSONL 读取遇到无法解析的行插入 `null` 元素** → 跳过。
20. **JSONL 写文件遇 `null` 元素抛 `NullReferenceException`** → 写为 `null`。
21. **JSONL 文件带 BOM、空列表写出 3 字节纯 BOM 文件、返回串 `\n` 与文件 `\r\n` 不一致** → 统一为无 BOM + `\n`。
22. **JSON5 的 `\x41`、`\v`、`\0`、字符串续行被原样保留** → 按规范解析。
23. **孤立代理项（如 `\uD800`）序列化成非法 JSON** → 转义为 `\uXXXX`，合法代理对（emoji）原样保留。

#### 互操作

24. **抽象类 / 接口 / 无公共无参构造的成员类型抛 `MissingMethodException`**（`Uri`、抽象基类成员）→ 跳过该成员。
25. **`object` 成员只认 JSON 对象且造出无内容的 `System.Object`**（数据丢失）→ 按实际类型映射。
26. **集合接口成员始终为 `null`** → 映射到 `List<T>` / `Dictionary<K,V>`。
27. **属性 getter 抛异常被包装成 `TargetInvocationException`** → 还原原始异常与堆栈。
28. **属性 setter 抛异常被包装成 `TargetInvocationException`** → 同上。
29. **`JsonData` 的 `(string)` 转换对畸形节点抛 `ArgumentOutOfRangeException`**（json 仅 1 个引号或空串）→ 安全返回。
30. **`JSON.ToJson(typeof(T))` 抛 `TargetInvocationException`** → 输出类型名字符串。
31. **JSON5 无引号键不支持 `$`** → 支持（并更正：JSON5 的 IdentifierName 不允许 `-`，带 `-` 的键仍按规范加引号）。

#### 输出合法性与反射

32. **`NaN` / `Infinity` 序列化成非法 JSON**（`{"d":NaN}`、`NaN`），且自己解析不回来（往返变 0）→ 纯 JSON 输出 `null`（与 `JSON.stringify` 一致），JSON5 保留字面量且可解析回来。
33. **数字前导零被原样保留** → 规范化，保证再序列化是合法 JSON。
34. **JSON5 的 `.5` / `5.` / `.5e1` / `5.e3` 解析失败** → 支持（纯 JSON 仍严格拒绝这些写法）。
35. **`MethodInfo` / `PropertyInfo` / `Assembly` / `Module` 序列化抛反射异常或误报「检测到循环引用」** → 按文本输出。
36. **JSON5 无引号键的 `\uXXXX` 转义不支持** → 支持。
37. **手工构造的深层 `JsonData`（`object` 成员递归）不受深度限制**，理论上仍可栈溢出 → 纳入 `MaxDepth` 保护。

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

This release focuses on bug fixes and interoperability: **35 additions/improvements and 37 fixes**, plus a new test project (`DanKeJson.Tests`) with 236 test cases.

### Added

**Parsing**

1. Leading UTF-8 BOM is ignored by `JSON.ToData` / `ToData<T>` and `JSON5.ToData` / `ToData<T>`.
2. Escaped characters in key names: `\"`, `\\`, `\/`, `\b`, `\f`, `\n`, `\r`, `\t`, `\uXXXX` (for JSON and JSON5 quoted keys; duplicate-key detection uses the decoded name).
3. Number literals are normalized: `01` → `1`, `007` → `7`, `-01` → `-1`, `00.5` → `0.5` (`0`, `-0`, `0.5`, `1e3` unchanged).
4. `JsonSettings.MaxDepth` (default 256, `<= 0` means unlimited) and `JsonSettings.DefaultMaxDepth`.
5. `JsonDepthLimitException`: a new public exception type with a `MaxDepth` property and a clear message.
6. Depth protection covers JSON parsing, JSON5 parsing, object graph serialization, object graph deserialization, `object` member recursion and self-referencing collections/objects.

**Deserialization**

7. `Dictionary<string,T>` members: values may be primitives, nested objects, lists or nested dictionaries; keys may be `string` / `int` / `enum` / `Guid`.
8. Top-level `Dictionary<string,T>` and `List<Dictionary<...>>` elements (sharing the same implementation as members).
9. Nullable value type members (`int?`, `bool?`, `Color?`, `DateTime?`, `Guid?`, ...); JSON `null` keeps them `null` instead of writing a default value.
10. `byte` members.
11. `char` members: accepts a JSON string (first character) or a numeric code point (`97` → `'a'`).
12. Enum members accept names: case-insensitive, flags syntax with commas, and numeric strings (`"1"`).
13. `object` members are mapped by the actual JSON type: `string` / `long` / `double` / `bool` / `List<object>` / `Dictionary<string,object>` / `null`.
14. `List<object>` elements and `Dictionary<*,object>` values use the same dynamic mapping.
15. Collection interface members are mapped to concrete implementations: `IEnumerable<T>`, `IList<T>`, `ICollection<T>`, `IReadOnlyList<T>`, `IReadOnlyCollection<T>` → `List<T>`; `IDictionary<K,V>`, `IReadOnlyDictionary<K,V>` → `Dictionary<K,V>`.
16. `JsonData` targets keep the original node: members, `List<JsonData>` elements, top-level `List<JsonData>` and dictionary values.
17. Uninstantiable member types (abstract classes, interfaces, types without a public parameterless constructor such as `Uri`) are skipped instead of throwing, while other members are still populated.

**Serialization**

18. Enumerables that are not `IList` (`Stack<T>`, `Queue<T>`, `HashSet<T>`, LINQ results, ...) serialize as arrays in enumeration order.
19. Reflection objects serialize as text: `Type`, `MemberInfo` (`MethodInfo` / `PropertyInfo` / ...), `Assembly`, `Module`.
20. `char` serializes as a string (`'a'` → `"a"`), and `JsonData` gained `implicit operator JsonData(char)` so a char no longer becomes its numeric code point.
21. Circular reference detection now also covers self-referencing collections (backed by the depth guard).

**JSON5**

22. Hexadecimal numbers: `0x10` / `0X10` / `-0x10` / `+0xff` (emitted as decimal).
23. Additional escapes: `\x` hex escape, `\v`, `\0` (kept literally when followed by a digit) and `\'`.
24. String continuation: `\` followed by LF / CRLF / CR / U+2028 / U+2029 is removed.
25. Decimal forms: `.5` → `0.5`, `5.` → `5`, `-.5` → `-0.5`, `.5e1` → `0.5e1`, `5.e3` → `5e3`.
26. Unquoted keys support `$` (parser and serializer agree).
27. Unquoted keys support `\uXXXX` identifier escapes (`{\u0061bc:1}` → key `abc`).
28. Comment removal is now a single string-aware scan: `//` and `/*...*/` inside string literals are preserved, and cleaning a 200k-character document dropped from 25 ms to 3 ms.

**JSONL**

29. Files are written as UTF-8 **without BOM** using `\n` line separators (matching the returned string).
30. `null` elements are written as JSON `null` (read back as a `Type.None` node).
31. Unparsable lines are skipped when reading instead of inserting `null` elements.

**Tests and documentation**

32. New `DanKeJson.Tests` project: xunit 2.9.3 + Microsoft.NET.Test.Sdk 17.14.1 + coverlet.collector 6.0.4, targeting `net10.0`, with **236 test cases**, added to `DanKeJson.sln`.
33. Test helpers: `TempWorkspace` (isolated temp directories) and `AssemblyInfo.cs` (parallel execution disabled because tests mutate `JsonSettings.MaxDepth` and the current culture).
34. API documentation additions: `JSON.ToData<T>` (exact member name matching, no type coercion), `JSON.ToJson` (NaN / Infinity → null) and `JSONL.ListToJson` (no BOM, `\n`, null elements, the target directory must exist).
35. New `CHANGELOG.md` plus README links; README / README_en version references and install commands updated to 1.6.0, and the date-format item marked as done.

### Fixed

**Member types and values**

1. Nullable value type members were never assigned → converted by the underlying type, and JSON `null` keeps them `null`.
2. `byte` / `char` members were never assigned → dedicated branches added.
3. `Dictionary<string,T>` members deserialized to an empty dictionary (silent data loss) → populated correctly.
4. Top-level enums serialized to member names (`Blue`, invalid JSON) → now emit the underlying number (`2`).
5. Enum members did not accept strings (`"Blue"` silently became the default 0) → names are parsed; unknown names keep the default.
6. Escaped quotes in key names failed the whole document → parsed correctly.
7. A leading BOM failed to parse → ignored.
8. JSON5 block comment removal deleted text inside strings (`"a/*b*/c"` → `"ac"`) → preserved.
9. JSON5 did not support `0x` hexadecimal numbers → supported.
10. `JsonData` `double` / `float` implicit conversions used the current culture (`"1.5"` became 15 under de-DE) → all numeric conversions now use `InvariantCulture` with explicit `NumberStyles`.

**Crashes and silent errors**

11. Deep nesting triggered an uncatchable `StackOverflowException` that terminated the process (JSON ≈3500 levels, JSON5 ≈3000, object graph serialization ≈900, deserialization ≈1200) → a catchable `JsonDepthLimitException`.
12. Serializing objects with a public indexer threw `TargetParameterCountException` (also hit by .NET 10 LINQ iterators).
13. Deserializing a document containing an `Item` key into a class with an indexer threw `TargetParameterCountException`.
14. Enumerables that are not `IList` were serialized as plain objects, producing results like `{"Count":3,"Capacity":3}` or `{"Comparer":{}}`.
15. `List<JsonData>` / `JSON.ToData<List<JsonData>>` threw `MissingMethodException` (`JsonData` has no parameterless constructor).
16. Top-level `Dictionary<string,T>` and `List<Dictionary<...>>` elements silently produced empty dictionaries.
17. Serializing a custom dictionary that yields a `null` key threw `ArgumentNullException` → written as an empty key (defensive).
18. `JsonData.HasKey(null)` and `data[null]` (read and write) threw `ArgumentNullException` → return false / null and ignore writes.
19. JSONL reading inserted `null` elements for unparsable lines → skipped.
20. JSONL writing threw `NullReferenceException` for `null` elements → written as `null`.
21. JSONL files carried a BOM, an empty list produced a 3-byte BOM-only file, and the returned string used `\n` while files used `\r\n` → unified to no BOM + `\n`.
22. JSON5 `\x41`, `\v`, `\0` and string continuation were kept literally → parsed per the specification.
23. Unpaired surrogates (for example `\uD800`) produced invalid JSON → escaped as `\uXXXX`, while valid pairs (emoji) are preserved.

**Interoperability**

24. Abstract classes, interfaces and types without a public parameterless constructor (`Uri`, abstract base members) threw `MissingMethodException` → the member is skipped.
25. `object` members only accepted JSON objects and produced an empty `System.Object` (data loss) → mapped by the actual JSON type.
26. Collection interface members were always `null` → mapped to `List<T>` / `Dictionary<K,V>`.
27. Exceptions thrown by property getters were wrapped in `TargetInvocationException` → the original exception and stack trace are restored.
28. Exceptions thrown by property setters were wrapped in `TargetInvocationException` → same as above.
29. The `JsonData` `(string)` conversion threw `ArgumentOutOfRangeException` for malformed nodes (a single quote or an empty `json`) → returns safely.
30. `JSON.ToJson(typeof(T))` threw `TargetInvocationException` → emits the type name as a string.
31. JSON5 unquoted keys did not support `$` → supported (and corrected: the JSON5 IdentifierName does not allow `-`, so keys containing `-` are still quoted).

**Output validity and reflection**

32. `NaN` / `Infinity` serialized to invalid JSON (`{"d":NaN}`, `NaN`) and could not be parsed back (round trip became 0) → plain JSON emits `null` (matching `JSON.stringify`) while JSON5 keeps the literals and parses them back.
33. Leading zeros in number literals were kept verbatim → normalized so re-serialization is always valid JSON.
34. JSON5 `.5` / `5.` / `.5e1` / `5.e3` failed to parse → supported (plain JSON still rejects these forms).
35. `MethodInfo` / `PropertyInfo` / `Assembly` / `Module` serialization threw reflection exceptions or falsely reported a circular reference → emitted as text.
36. JSON5 unquoted keys did not support `\uXXXX` escapes → supported.
37. Hand-built deep `JsonData` values (recursion through `object` members) were not covered by the depth limit and could still overflow the stack → now guarded by `MaxDepth`.

### Changed

- Top-level enums now serialize to numbers (`2`) instead of member names (`Blue`)
- `char` now serializes to a string (`"a"`) instead of its code point (`97`)
- `NaN` / `Infinity` serialize to `null` in plain JSON (matching `JSON.stringify`); JSON5 keeps the literals
- Nesting deeper than `JsonSettings.MaxDepth` (default 256) now throws `JsonDepthLimitException`; set it to `<= 0` to remove the limit (at the cost of the previous crash risk)
- Number literals are normalized: `JSON.ToData("01").json` changed from `"01"` to `"1"`
- JSONL files no longer carry a BOM and use `\n`
- Nullable members stay `null` on JSON `null` instead of receiving the default value
- Reflection objects (`Type`, ...) are emitted as text instead of throwing
- `JSONL.ListToJson` requires the target directory to exist and otherwise throws `DirectoryNotFoundException` (a .NET convention, now documented)
- Member matching remains exact with no implicit type coercion (`{"i":"5"}` will not populate an `int` member), now documented and covered by tests

### Platforms

net10.0 / net9.0 / net8.0 / net7.0 / net6.0 / net5.0 / netstandard2.1 / netcoreapp3.0 / netcoreapp3.1

## [1.5.1] - 2026-08-10

- Support for entity classes defined with public fields (LitJSON style, no `{ get; set; }` required)
- Deserialization support for arrays, `decimal`, `DateTime` / `DateTimeOffset` / `TimeSpan` / `Guid`, enums, `List<T>` and `IDictionary`
- Serialization support for lists, arrays, dictionaries and enums, plus circular reference detection
- JSONL write guard for empty file paths

> Changes before 1.5.1 are not documented.
