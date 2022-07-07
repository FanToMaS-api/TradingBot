# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
RUN apt-get update && apt-get install -y libgdiplus

COPY ./src/Services/TelegramServiceWeb/TelegramServiceWeb.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./src/Services/TelegramServiceWeb/TelegramServiceWeb.csproj -c Release -o out

# RUntime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build-env /app/out .
RUN apt-get update && apt-get install -y libgdiplus
ENTRYPOINT ["dotnet", "TelegramServiceWeb.dll"]
