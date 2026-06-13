#!/bin/bash
# Runs on the Raspberry Pi via systemd timer (as root).
# Checks GitHub for the latest Pi release, and if newer than what's installed,
# downloads and deploys it, then (re)launches the app in the desktop session.
set -e

REPO="emma-cogensoft/HomeAutomation"
APP_DIR="/opt/homeautomation"
APP_USER="pi"
VERSION_FILE="$APP_DIR/.deployed-release"
LOG_DIR="/var/log/homeautomation"

LATEST_TAG=$(curl -sf "https://api.github.com/repos/$REPO/releases/latest" | grep '"tag_name"' | sed -E 's/.*"tag_name": *"([^"]+)".*/\1/')

if [ -z "$LATEST_TAG" ]; then
    echo "Failed to fetch latest release tag"
    exit 1
fi

CURRENT_TAG=""
if [ -f "$VERSION_FILE" ]; then
    CURRENT_TAG=$(cat "$VERSION_FILE")
fi

if [ "$LATEST_TAG" == "$CURRENT_TAG" ]; then
    echo "Already up to date ($CURRENT_TAG)"
    exit 0
fi

echo "Updating from '$CURRENT_TAG' to '$LATEST_TAG'..."

TMP_DIR=$(mktemp -d)
trap 'rm -rf "$TMP_DIR"' EXIT

ASSET_URL="https://github.com/$REPO/releases/download/$LATEST_TAG/homeautomation-desktop-linux-arm.tar.gz"
curl -sfL "$ASSET_URL" -o "$TMP_DIR/release.tar.gz"

echo "Stopping running app..."
pkill -f "$APP_DIR/HomeAutomation.Desktop" || true

# Preserve config that isn't part of the release
cp "$APP_DIR/appsettings.json" "$TMP_DIR/appsettings.json" 2>/dev/null || true

find "$APP_DIR" -mindepth 1 -delete
tar -xzf "$TMP_DIR/release.tar.gz" -C "$APP_DIR"

if [ -f "$TMP_DIR/appsettings.json" ]; then
    cp "$TMP_DIR/appsettings.json" "$APP_DIR/appsettings.json"
fi

chmod +x "$APP_DIR/HomeAutomation.Desktop"
chown -R "$APP_USER:$APP_USER" "$APP_DIR"
echo "$LATEST_TAG" > "$VERSION_FILE"

mkdir -p "$LOG_DIR"
chown "$APP_USER:$APP_USER" "$LOG_DIR"

echo "Launching app in desktop session..."
sudo -u "$APP_USER" DISPLAY=:0 XAUTHORITY="/home/$APP_USER/.Xauthority" \
    "$APP_DIR/HomeAutomation.Desktop" >> "$LOG_DIR/app.log" 2>&1 &
disown

echo "Deployed $LATEST_TAG"
