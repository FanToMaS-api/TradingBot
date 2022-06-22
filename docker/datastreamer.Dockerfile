# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY ./src/Services/Datastreamer/Datastreamer.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish ./src/Services/Datastreamer/Datastreamer.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Datastreamer.dll"]
