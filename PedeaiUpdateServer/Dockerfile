# ── Build ─────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PedeaiUpdateServer.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# ── Runtime ───────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p Data/packages
EXPOSE 5001
ENTRYPOINT ["dotnet", "PedeaiUpdateServer.dll"]
