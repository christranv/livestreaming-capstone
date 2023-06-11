FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base  
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY "BuildingBlocks/Metrics/Metrics.csproj" "BuildingBlocks/Metrics/Metrics.csproj"
COPY "WebApps/WebStatus/WebStatus.csproj" "WebApps/WebStatus/WebStatus.csproj"

RUN dotnet restore "WebApps/WebStatus/WebStatus.csproj"
COPY . .

#WORKDIR "src/WebApps/WebStatus"
#RUN dotnet build WebStatus.csproj -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "WebStatus.dll"]

WORKDIR /src/WebApps/WebStatus
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WebStatus.dll"]
