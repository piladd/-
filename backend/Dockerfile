# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем решение и проекты
COPY Messenger.sln .
COPY Messenger.API/Messenger.API.csproj Messenger.API/
COPY Messenger.Application/Messenger.Application.csproj Messenger.Application/
COPY Messenger.Domain/Messenger.Domain.csproj Messenger.Domain/
COPY Messenger.Persistence/Messenger.Persistence.csproj Messenger.Persistence/
COPY Messenger.Security/Messenger.Security.csproj Messenger.Security/
COPY Messenger.Tests/Messenger.Tests.csproj Messenger.Tests/

# Восстанавливаем зависимости
RUN dotnet restore

# Копируем всё остальное и публикуем
COPY . .
WORKDIR /src/Messenger.API
RUN dotnet publish -c Release -o /app/out

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем выходные артефакты из билд-стадии
COPY --from=build /app/out .

# Порт, если у вас на API слушает 80/443 (можете убрать, если не нужно)
EXPOSE 80

ENTRYPOINT ["dotnet", "Messenger.API.dll"]
