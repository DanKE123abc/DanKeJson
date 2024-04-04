set output=./publish
if exist "%output%" rd /S /Q "%output%"
dotnet build ./DanKeJson/DanKeJson.csproj
xcopy /E /I /Y ".\DanKeJson\lib\" "%output%\DanKeJson\lib\"
copy /Y ".\DanKeJson\DanKeJson.nuspec" "%output%\DanKeJson\DanKeJson.nuspec"
copy /Y ".\DanKeJson\LICENSE" "%output%\DanKeJson\LICENSE"
nuget pack ./publish/DanKeJson/DanKeJson.nuspec -Prop Configuration=Release