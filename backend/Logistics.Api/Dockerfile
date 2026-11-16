
# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY Logistics.Api/Logistics.Api.csproj Logistics.Api/

RUN dotnet restore Logistics.Api/Logistics.Api.csproj

COPY Logistics.Api/ Logistics.Api/

RUN dotnet publish Logistics.Api/Logistics.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "Logistics.Api.dll"]
