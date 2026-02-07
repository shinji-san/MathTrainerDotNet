# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Project files copy and dependencies restore
COPY MathTrainerDotNet/*.csproj ./MathTrainerDotNet/
RUN dotnet restore MathTrainerDotNet/MathTrainerDotNet.csproj

# Source code copy and build
COPY MathTrainerDotNet/ ./MathTrainerDotNet/

# Publish
WORKDIR /src/MathTrainerDotNet
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage - with nginx
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# install nginx and supervisor
RUN apt-get update && apt-get install -y --no-install-recommends \
    nginx \
    supervisor \
    gosu \
    && rm -rf /var/lib/apt/lists/*

# Create non-root user
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

# Create directories
RUN mkdir -p /app/data /var/log/supervisor

# Copy app
WORKDIR /app
COPY --from=build /app/publish .

# copy nginx configuration
COPY nginx/nginx.conf /etc/nginx/nginx.conf
RUN rm -f /etc/nginx/sites-enabled/default /etc/nginx/conf.d/default.conf

# Supervisor configuration
RUN cat > /etc/supervisor/conf.d/supervisord.conf << 'EOF'
[supervisord]
nodaemon=true
user=root
logfile=/var/log/supervisor/supervisord.log
pidfile=/var/run/supervisord.pid

[program:nginx]
command=/usr/sbin/nginx -g "daemon off;"
autostart=true
autorestart=true
stdout_logfile=/dev/stdout
stdout_logfile_maxbytes=0
stderr_logfile=/dev/stderr
stderr_logfile_maxbytes=0

[program:dotnet]
command=gosu appuser dotnet /app/MathTrainerDotNet.dll
directory=/app
autostart=true
autorestart=true
stdout_logfile=/dev/stdout
stdout_logfile_maxbytes=0
stderr_logfile=/dev/stderr
stderr_logfile_maxbytes=0
environment=ASPNETCORE_URLS="http://+:5000",ASPNETCORE_ENVIRONMENT="Production",ConnectionStrings__DefaultConnection="Data Source=/app/data/mathtrainer.db"
EOF

# Entrypoint script
RUN cat > /usr/local/bin/docker-entrypoint.sh << 'EOF'
#!/bin/sh
set -e

# Set data directory permissions
mkdir -p /app/data
chown -R appuser:appgroup /app/data

# Start supervisor (manages nginx + dotnet)
exec /usr/bin/supervisord -c /etc/supervisor/conf.d/supervisord.conf
EOF
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# Expose port (nginx)
EXPOSE 80

ENTRYPOINT ["docker-entrypoint.sh"]
