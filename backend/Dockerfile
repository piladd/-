# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Messenger.sln ./
COPY Messenger.API/*.csproj Messenger.API/
COPY Messenger.Application/*.csproj Messenger.Application/
COPY Messenger.Domain/*.csproj Messenger.Domain/
COPY Messenger.Infrastructure/*.csproj Messenger.Infrastructure/
COPY Messenger.Security/*.csproj Messenger.Security/
RUN dotnet restore

COPY . ./
RUN dotnet publish Messenger.API/Messenger.API.csproj -c Release -o /app/out

# Этап рантайма
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

# Копируем wait-for-it.sh из исходной директории сборки
COPY wait-for-it.sh ./wait-for-it.sh
RUN chmod +x wait-for-it.sh

ENTRYPOINT ["dotnet", "Messenger.API.dll"]
