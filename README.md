# DanKeJson
![NuGet](https://img.shields.io/nuget/v/DanKeJson.svg)

DanKeJson is a simple *.Net* library to handle conversions from and to JSON (JavaScript Object Notation) strings.



## Quick-Start

[Hello DanKeJson](./Docs/DanKeJson.md)

[Json to JsonData](./Docs/QuickStart/Json2JsonData.md)

[Json to Object](./Docs/QuickStart/Json2Object.md)

[JsonData to Json](./Docs/QuickStart/JsonData2Json.md)

[Object to Json](./Docs/QuickStart/Object2Json.md)



## API

[JSON](./Docs/API/JSON.md)

[JsonData](./Docs/API/JsonData.md)

[JsonData.Type](./Docs/API/JsonData.Type.md)



## Publishing

**Windows**

```shell
./publish.bat
```



## Using DanKeJson from an application

**[Download package](https://www.nuget.org/api/v2/package/DanKeJson/1.0.4)**

### Package manager

```shell
NuGet\Install-Package DanKeJson -Version 1.0.4
```

### .NET CLI

```shell
dotnet add package DanKeJson --version 1.0.4
```

### PackageReference

```xaml
<PackageReference Include="DanKeJson" Version="1.0.4" />
```

### Paket CLI

```shell
paket add DanKeJson --version 1.0.4
```

### Script & Interactive

```c#
#r "nuget: DanKeJson, 1.0.4"
```

### Cake

```C#
// Install DanKeJson as a Cake Addin
#addin nuget:?package=DanKeJson&version=1.0.4

// Install DanKeJson as a Cake Tool
#tool nuget:?package=DanKeJson&version=1.0.4
```

Alternatively, just copy the whole tree of files under `./DanKeJson` to your own project's source tree and integrate it with your development environment.



## Requirements

DanKeJson currently targets and supports

- .NET 8.0
- .NET 7.0
- .NET 6.0
- .NET 5.0
- .NET Standard 2.1
- .NETcoreapp3.1
- .NETcoreapp3.0
- Mono



## License

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

