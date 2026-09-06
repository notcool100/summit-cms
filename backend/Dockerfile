# Build context is the `backend/` directory (see .dockerignore alongside this file).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore src/Host/SummitCms.Api/SummitCms.Api.csproj
RUN dotnet publish src/Host/SummitCms.Api/SummitCms.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# curl is required for Coolify's container healthcheck (it execs curl/wget inside the
# container to hit /health) - the base aspnet image doesn't ship either by default.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && useradd --create-home --shell /bin/bash appuser \
    && mkdir -p /app/App_Data/uploads \
    && chown -R appuser:appuser /app

COPY --from=build --chown=appuser:appuser /app/publish .

USER appuser

ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 8080

# The app applies pending EF Core migrations and runs the idempotent content
# seeder on startup (see Program.cs) - no separate migration step needed.
ENTRYPOINT ["dotnet", "SummitCms.Api.dll"]
