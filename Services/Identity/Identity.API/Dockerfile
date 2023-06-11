FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY "Services/Identity/Identity.API/Identity.API.csproj" "Services/Identity/Identity.API/Identity.API.csproj"
COPY "BuildingBlocks/BuildingBlocks.csproj" "BuildingBlocks/BuildingBlocks.csproj"
COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "Services/Identity/Identity.API/Identity.API.csproj"
COPY . .

WORKDIR /src/Services/Identity/Identity.API
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.API.dll"]
