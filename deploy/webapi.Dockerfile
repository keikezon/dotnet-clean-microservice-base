# Multi-stage build for Identity.API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet restore ./CleanMicroserviceBase.sln
RUN dotnet publish ./src/Services/Identity/Identity.API/Identity.API.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /out ./
ENTRYPOINT ["dotnet", "Identity.API.dll", "--urls", "http://0.0.0.0:8080"]
