# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY MathTrainerDotNet/*.csproj ./MathTrainerDotNet/
RUN dotnet restore MathTrainerDotNet/MathTrainerDotNet.csproj

COPY MathTrainerDotNet/ ./MathTrainerDotNet/

WORKDIR /src/MathTrainerDotNet
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    gosu \
    && rm -rf /var/lib/apt/lists/*

RUN groupadd -r appgroup && useradd -r -g appgroup appuser

RUN mkdir -p /app/data

COPY --from=build /app/publish .

COPY docker-entrypoint.sh /usr/local/bin/
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/mathtrainer.db"

# Entrypoint (starts as root, then change to appuser)
ENTRYPOINT ["docker-entrypoint.sh"]
