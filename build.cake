var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var outputDir = "./artifacts";
var projectDir = "./DanKeJson/";

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
    DotNetPack(projectDir,
        new DotNetPackSettings {
            Configuration = "Release",
            NoRestore = true,
            NoBuild = true,
            IncludeSymbols = true,
            OutputDirectory = outputDir,
        }
    );
});

// 默认任务链
Task("Default")
    .IsDependentOn("Package");

// 执行入口
RunTarget(target);