# Build image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app
RUN apt-get update && apt-get install -y libgdiplus

COPY ./tests/SignalsSender/SignalsSender.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./tests/SignalsSender/SignalsSender.csproj -c Release -o out

# RUntime image
FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=build-env /app/out .
RUN apt-get update && apt-get install -y libgdiplus
ENTRYPOINT ["dotnet", "SignalsSender.dll"]
