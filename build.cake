var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var outputDir = "./artifacts";
var projectDir = "./DanKeJson/";
var suffix = Argument("suffix", "");


// 清理发布目录
Task("Clean")
    .Does(() =>
{
    if (DirectoryExists(outputDir))
    {
        DeleteDirectory(outputDir, new DeleteDirectorySettings { Recursive = true, Force = true });
    }
    CreateDirectory(outputDir);
});

// 编译项目
Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetBuild(projectDir, new DotNetBuildSettings
    {
        Configuration = configuration,
        NoIncremental = true,
    });
});

// 打包 NuGet 包
Task("Package")
    .IsDependentOn("Build")
    .Does(() => 
{
    var buildSettings = new DotNetPackSettings();
    var version = XmlPeek(projectDir + "DanKeJson.csproj", "/Project/PropertyGroup/Version");
    if(suffix != "")
    {
        buildSettings = new DotNetPackSettings()
            {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            IncludeSymbols = true,
            OutputDirectory = outputDir,
            MSBuildSettings = new DotNetMSBuildSettings()
            .WithProperty("Version", $"{version}-{suffix}"),
            };
    }
    else
    {
        buildSettings = new DotNetPackSettings {
            Configuration = configuration,
            NoRestore = true,
            NoBuild = true,
            IncludeSymbols = true,
            OutputDirectory = outputDir,
            };
    }
    DotNetPack(projectDir,buildSettings);
});

// 默认任务链
Task("Default")
    .IsDependentOn("Package");

// 执行入口
RunTarget(target);