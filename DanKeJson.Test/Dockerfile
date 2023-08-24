FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DanKeJson.Test/DanKeJson.Test.csproj", "DanKeJson.Test/"]
RUN dotnet restore "DanKeJson.Test/DanKeJson.Test.csproj"
COPY . .
WORKDIR "/src/DanKeJson.Test"
RUN dotnet build "DanKeJson.Test.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DanKeJson.Test.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DanKeJson.Test.dll"]
