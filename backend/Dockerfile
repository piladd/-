# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Messenger.sln .
COPY Messenger.API/*.csproj Messenger.API/
COPY Messenger.Application/*.csproj Messenger.Application/
COPY Messenger.Domain/*.csproj Messenger.Domain/
COPY Messenger.Infrastructure/*.csproj Messenger.Infrastructure/
COPY Messenger.Security/*.csproj Messenger.Security/
COPY Messenger.Tests/*.csproj Messenger.Tests/

RUN dotnet restore

COPY . .
RUN dotnet publish Messenger.API/Messenger.API.csproj -c Release -o /app/out

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "Messenger.API.dll"]
