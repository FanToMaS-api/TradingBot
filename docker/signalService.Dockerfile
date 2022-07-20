# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./tests/SignalsSender/SignalsSender.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./tests/SignalsSender/SignalsSender.csproj -c Release -o out

# RUntime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build-env /app/out .
RUN apt-get update && apt-get install -y libgdiplus
ENTRYPOINT ["dotnet", "SignalsSender.dll"]
