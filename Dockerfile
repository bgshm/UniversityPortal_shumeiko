# ---== Этап 1: Сборка (Build) ==---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем .sln и .csproj
COPY *.sln .
COPY UniversityPortal_shumeiko/*.csproj UniversityPortal_shumeiko/

# Восстанавливаем зависимости
RUN dotnet restore "UniversityPortal_shumeiko.sln"

# Копируем весь остальной исходный код
COPY . .

# Публикуем проект
WORKDIR /src/UniversityPortal_shumeiko
RUN dotnet publish "UniversityPortal_shumeiko.csproj" -c Release -o /app/publish

# ---== Этап 2: Финальный (Final) ==---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "UniversityPortal_shumeiko.dll"]
