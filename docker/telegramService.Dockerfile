# Build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY ./src/Services/TelegramService/TelegramService.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./src/Services/TelegramService/TelegramService.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "TelegramService.dll"]
