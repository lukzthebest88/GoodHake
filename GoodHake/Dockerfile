# Basis-Image f�r .NET 8 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Basis-Image f�r .NET SDK (wird nur f�r den Build genutzt)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Projektdateien kopieren & Wiederherstellen der Abh�ngigkeiten
COPY ["GoodHake.csproj", "GoodHake/"]
WORKDIR "/src/GoodHake"
RUN dotnet restore "GoodHake.csproj"

# Alle Dateien kopieren & Projekt bauen
COPY . .
RUN dotnet build "GoodHake.csproj" -c Release -o /app/build

# Ver�ffentlichen
FROM build AS publish
RUN dotnet publish "GoodHake.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Finales Image mit Runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GoodHake.dll"]
