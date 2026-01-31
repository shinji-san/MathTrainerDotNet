#!/bin/sh
set -e

# Sicherstellen, dass das Datenverzeichnis existiert und beschreibbar ist
mkdir -p /app/data
chown -R appuser:appgroup /app/data

# Als appuser die Anwendung starten
exec gosu appuser dotnet MathTrainerDotNet.dll
