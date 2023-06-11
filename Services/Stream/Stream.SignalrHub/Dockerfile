FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY "BuildingBlocks/BuildingBlocks.csproj" "BuildingBlocks/BuildingBlocks.csproj"
COPY "Services/Stream/Stream.SignalrHub/Stream.SignalrHub.csproj" "Services/Stream/Stream.SignalrHub/Stream.SignalrHub.csproj"
COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "Services/Stream/Stream.SignalrHub/Stream.SignalrHub.csproj"
COPY . .

WORKDIR /src/Services/Stream/Stream.SignalrHub
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Stream.SignalrHub.dll"]
