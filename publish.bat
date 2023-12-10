set output=./publish
if exist "%output%" rd /S /Q "%output%"
dotnet build ./DanKeJson/DanKeJson.csproj
xcopy /E /I /Y ".\DanKeJson" "%output%\DanKeJson"
nuget pack ./publish/DanKeJson/DanKeJson.nuspec -Prop Configuration=Release