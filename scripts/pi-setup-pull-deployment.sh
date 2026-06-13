#!/bin/bash
# One-time setup on the Raspberry Pi for pull-based deployment.
# Run as a user with sudo access, e.g.:
#   bash pi-setup-pull-deployment.sh
set -e

SCRIPT_DIR="/opt/homeautomation-scripts"
REPO_RAW="https://raw.githubusercontent.com/emma-cogensoft/HomeAutomation/main/scripts"

echo "==> Creating $SCRIPT_DIR..."
sudo mkdir -p "$SCRIPT_DIR"

echo "==> Downloading update script and systemd units..."
sudo curl -sfL "$REPO_RAW/pi-pull-update.sh" -o "$SCRIPT_DIR/pi-pull-update.sh"
sudo chmod +x "$SCRIPT_DIR/pi-pull-update.sh"

sudo curl -sfL "$REPO_RAW/homeautomation-update.service" -o /etc/systemd/system/homeautomation-update.service
sudo curl -sfL "$REPO_RAW/homeautomation-update.timer" -o /etc/systemd/system/homeautomation-update.timer

echo "==> Enabling and starting timer..."
sudo systemctl daemon-reload
sudo systemctl enable --now homeautomation-update.timer

echo "==> Running first update now..."
sudo systemctl start homeautomation-update.service

echo "==> Done. Check status with:"
echo "    systemctl status homeautomation-update.timer"
echo "    journalctl -u homeautomation-update.service -f"
