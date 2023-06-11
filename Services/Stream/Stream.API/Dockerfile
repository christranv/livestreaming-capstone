FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY "BuildingBlocks/BuildingBlocks.csproj" "BuildingBlocks/BuildingBlocks.csproj"
COPY "Services/Stream/Stream.Domain/Stream.Domain.csproj" "Services/Stream/Stream.Domain/Stream.Domain.csproj"
COPY "Services/Stream/Stream.Infrastructure/Stream.Infrastructure.csproj" "Services/Stream/Stream.Infrastructure/Stream.Infrastructure.csproj"
COPY "Services/Stream/Stream.API/Stream.API.csproj" "Services/Stream/Stream.API/Stream.API.csproj"
COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "Services/Stream/Stream.API/Stream.API.csproj"
COPY . .

WORKDIR /src/Services/Stream/Stream.API
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Stream.API.dll"]
