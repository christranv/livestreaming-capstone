FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY "BuildingBlocks/BuildingBlocks.csproj" "BuildingBlocks/BuildingBlocks.csproj"
COPY "Services/Event/Event.API/Event.API.csproj" "Services/Event/Event.API/Event.API.csproj"
COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "Services/Event/Event.API/Event.API.csproj"
COPY . .

WORKDIR /src/Services/Event/Event.API
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Event.API.dll"]
