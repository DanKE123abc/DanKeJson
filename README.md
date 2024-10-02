# DanKeJson

<u>**中文**</u> | [**English**](./README_en.md)

![NuGet](https://img.shields.io/nuget/v/DanKeJson.svg)

DanKeJson 是一个.Net平台上的精简小巧的 JSON 类库，用于处理 JSON (JavaScript Object Notation) 字符串之间的转换。

## 快速开始

[Hello DanKeJson](./Docs/DanKeJson.md)

[Json to JsonData](./Docs/QuickStart/Json2JsonData.md)

[Json to Object](./Docs/QuickStart/Json2Object.md)

[JsonData to Json](./Docs/QuickStart/JsonData2Json.md)

[Object to Json](./Docs/QuickStart/Object2Json.md)

## 特性

DanKeJson 为了方便开发使用，添加了许多特性，包括但不限于：

- 以 [JsonData]([JsonData](./Docs/API/JsonData.md)) 类传递数据
- 隐式转换操作符
- 允许数组、对象的多余逗号
- 字符串允许使用单引号
- 允许注释*
- 将未经定义的值转换为null
- 键名允许不使用引号

更多特性相关内容请看：[**Hello DanKeJson**](./Docs/DanKeJson.md)

> *: 当前版本的 DanKeJson解析器部分中尚未支持注释，注释支持由CommentParser类通过正则表达式忽略，可能会严重影响解析性能。

## 将来要做的事...

反序列化部分：

- [ ]  解析器支持单行注释与多行注释
- [ ]  支持日期格式
- [ ]  支持更多 .net 版本
- [X]  键名无需引号
- [X]  字符串支持单引号
- [X]  允许数组、对象的多余逗号

序列化部分：

- [ ]  支持序列化为JSON5格式

杂项：

- [ ]  支持序列化反序列化MessagePack
- [ ]  使用Simd技术加快反序列化

## API

[JSON](./Docs/API/JSON.md)

[JSON5](./Docs/API/JSON5.md)

[JsonData](./Docs/API/JsonData.md)

[JsonData.Type](./Docs/API/JsonData.Type.md)

## 打包为Nuget发行包

**Windows**

```shell
./publish.bat
```

## 在你的应用中安装 DanKeJson

**[点击下载](https://www.nuget.org/api/v2/package/DanKeJson/1.3.3)**

### Package manager

```shell
NuGet\Install-Package DanKeJson -Version 1.3.3
```

### .NET CLI

```shell
dotnet add package DanKeJson --version 1.3.3
```

### PackageReference

```xaml
<PackageReference Include="DanKeJson" Version="1.3.3" />
```

### Paket CLI

```shell
paket add DanKeJson --version 1.3.3
```

### Script & Interactive

```c#
#r "nuget: DanKeJson, 1.3.3"
```

### Cake

```C#
// Install DanKeJson as a Cake Addin
#addin nuget:?package=DanKeJson&version=1.3.3

// Install DanKeJson as a Cake Tool
#tool nuget:?package=DanKeJson&version=1.3.3
```

或者，只需复制目录`./publish/DanKeJson`到您自己项目的源代码树中，并将其与您的开发环境集成。

## 开发平台

DanKeJson 目前支持的平台：

- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET 5.0
- .NET Standard 2.1
- .NETcoreapp3.1
- .NETcoreapp3.0
- Godot (.Net)
- Mono
- Unity

## 开源许可协议

[中文翻译版](https://github.com/DanKE123abc/DanKE123abc/blob/main/%5B%E4%B8%AD%5D%20MIT%20License.txt)

```
MIT License

Copyright (c) 2023 - present DanKe

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## 特别鸣谢

本项目使用 Rider 开发，感谢 **JetBrains** 对 DanKeJson 的支持。

<img src="https://resources.jetbrains.com/storage/products/company/brand/logos/jb_beam.png" alt="JetBrains Logo (Main) logo." width=100 height=100>

本项目所有Markdown文件使用 [**Hypora**](https://github.com/DanKE123abc/Hypora) 书写。

## 参考项目

[litjson - C#](https://github.com/LitJSON/litjson)