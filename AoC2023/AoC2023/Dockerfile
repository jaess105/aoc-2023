FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AoC2023/AoC2023.csproj", "AoC2023/"]
RUN dotnet restore "AoC2023/AoC2023.csproj"
COPY . .
WORKDIR "/src/AoC2023"
RUN dotnet build "AoC2023.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AoC2023.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AoC2023.dll"]
