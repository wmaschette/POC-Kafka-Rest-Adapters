FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine3.14
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "kafka-producer.dll"]