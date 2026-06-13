#!/bin/bash
# One-time setup on the Raspberry Pi for pull-based deployment.
# Run as a user with sudo access, e.g.:
#   bash pi-setup-pull-deployment.sh
set -e

SCRIPT_DIR="/opt/homeautomation-scripts"
REPO_RAW="https://raw.githubusercontent.com/emma-cogensoft/HomeAutomation/main/scripts"
PI_USER="pi"

echo "==> Removing old systemd service (GUI app cannot run headless)..."
sudo systemctl disable --now homeautomation.service 2>/dev/null || true
sudo rm -f /etc/systemd/system/homeautomation.service

echo "==> Creating $SCRIPT_DIR..."
sudo mkdir -p "$SCRIPT_DIR"

echo "==> Downloading update script and systemd units..."
sudo curl -sfL "$REPO_RAW/pi-pull-update.sh" -o "$SCRIPT_DIR/pi-pull-update.sh"
sudo chmod +x "$SCRIPT_DIR/pi-pull-update.sh"

sudo curl -sfL "$REPO_RAW/homeautomation-update.service" -o /etc/systemd/system/homeautomation-update.service
sudo curl -sfL "$REPO_RAW/homeautomation-update.timer" -o /etc/systemd/system/homeautomation-update.timer

echo "==> Installing autostart entry for desktop session..."
AUTOSTART_DIR="/home/${PI_USER}/.config/autostart"
sudo -u "$PI_USER" mkdir -p "$AUTOSTART_DIR"
sudo -u "$PI_USER" curl -sfL "$REPO_RAW/homeautomation-desktop.desktop" -o "$AUTOSTART_DIR/homeautomation-desktop.desktop"

echo "==> Enabling and starting timer..."
sudo systemctl daemon-reload
sudo systemctl enable --now homeautomation-update.timer

echo "==> Running first update now..."
sudo systemctl start homeautomation-update.service

echo "==> Done. Check status with:"
echo "    systemctl status homeautomation-update.timer"
echo "    journalctl -u homeautomation-update.service -f"
echo ""
echo "==> NOTE: Ensure auto-login is enabled in raspi-config:"
echo "    sudo raspi-config -> System Options -> Boot / Auto Login -> Desktop Autologin"
echo "    The dashboard will launch automatically after the next reboot/login,"
echo "    or immediately if a desktop session is already active (15s delay)."
