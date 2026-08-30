# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Project files copy and dependencies restore.
# Directory.Build.props must be copied before the restore: it carries the
# NuGet audit gate that escalates NU1901-NU1904 to errors. Without it the
# image build would restore unaudited, so a vulnerable package could still be
# baked into the published image even though CI rejects it.
COPY Directory.Build.props ./
COPY MathTrainerDotNet/*.csproj ./MathTrainerDotNet/
RUN dotnet restore MathTrainerDotNet/MathTrainerDotNet.csproj

# Source code copy and build
COPY MathTrainerDotNet/ ./MathTrainerDotNet/

# Publish
WORKDIR /src/MathTrainerDotNet
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage - with nginx
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# install nginx and supervisor.
# gosu is deliberately absent: nothing in this image switches user at runtime
# any more, so a setuid-style helper would only be an escalation primitive.
RUN apt-get update && apt-get install -y --no-install-recommends \
    nginx \
    supervisor \
    && rm -rf /var/lib/apt/lists/*

# The runtime base image already ships a non-root user (app, UID/GID 1654,
# exported as APP_UID) with a real home directory. Reuse it instead of
# creating a second one -- a system account from "useradd -r" gets a home
# directory entry that is never created, which leaves $HOME unwritable.

# Create directories and hand them to that user. nginx and supervisor both
# ship root-owned log directories, and neither can write there once the
# master process is unprivileged.
RUN mkdir -p /app/data /var/log/supervisor \
    && chown -R $APP_UID:$APP_UID /app/data /var/log/supervisor /var/log/nginx

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
logfile=/var/log/supervisor/supervisord.log
pidfile=/tmp/supervisord.pid

[program:nginx]
command=/usr/sbin/nginx -g "daemon off;"
autostart=true
autorestart=true
stdout_logfile=/dev/stdout
stdout_logfile_maxbytes=0
stderr_logfile=/dev/stderr
stderr_logfile_maxbytes=0

[program:dotnet]
command=dotnet /app/MathTrainerDotNet.dll
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

# /app/data is created and chowned to UID 1654 at build time, so a *new* named
# volume inherits that ownership. A volume written by an older release of this
# image does not -- that one ran as root and left the database owned by a
# different uid. This container cannot repair that, so say so here instead of
# crash-looping on "attempt to write a readonly database" a second later.
if [ ! -w /app/data ]; then
    echo "FATAL: /app/data is not writable by uid $(id -u)." >&2
    echo "  This image no longer runs as root. If you are upgrading from an" >&2
    echo "  earlier version, hand the existing volume to the new user once:" >&2
    echo "    docker run --rm -v mathtrainer-data:/data alpine chown -R 1654:1654 /data" >&2
    exit 1
fi

# Start supervisor (manages nginx + dotnet)
exec /usr/bin/supervisord -c /etc/supervisor/conf.d/supervisord.conf
EOF
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# Drop privileges for everything from here on: supervisor, the nginx master
# and the app itself all run as this user. Numeric so that a Kubernetes
# runAsNonRoot check can verify it without resolving /etc/passwd.
USER $APP_UID

# Expose port (nginx). 8080 rather than 80 because an unprivileged process
# cannot bind a port below 1024.
EXPOSE 8080

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
