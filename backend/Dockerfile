# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копируем решение и проекты
COPY Messenger.sln ./
COPY Messenger.API/*.csproj Messenger.API/
COPY Messenger.Application/*.csproj Messenger.Application/
COPY Messenger.Domain/*.csproj Messenger.Domain/
COPY Messenger.Infrastructure/*.csproj Messenger.Infrastructure/
COPY Messenger.Security/*.csproj Messenger.Security/
COPY Messenger.Tests/*.csproj Messenger.Tests/

# Восстановление зависимостей
RUN dotnet restore

# Копируем весь исходный код
COPY . .

# (Опционально) Запускаем тесты
#RUN dotnet test Messenger.Tests/Messenger.Tests.csproj --no-build --verbosity normal

# Публикуем приложение
RUN dotnet publish Messenger.API/Messenger.API.csproj -c Release -o /app/out

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Messenger.API.dll"]
