#!/bin/bash
# Runs on the Raspberry Pi to configure the systemd service.
# Called automatically by deploy.ps1, but can also be run manually:
#   bash setup.sh [pi-path] [pi-user]
set -e

PI_PATH="${1:-/opt/homeautomation}"
PI_USER="${2:-pi}"

echo "==> Setting execute permission on binary..."
chmod +x "$PI_PATH/HomeAutomation.Web"

echo "==> Writing systemd service file..."
sudo tee /etc/systemd/system/homeautomation.service > /dev/null <<EOF
[Unit]
Description=Home Energy Dashboard
After=network.target

[Service]
Type=simple
User=$PI_USER
WorkingDirectory=$PI_PATH
ExecStart=$PI_PATH/HomeAutomation.Web
Restart=on-failure
RestartSec=10

[Install]
WantedBy=multi-user.target
EOF

echo "==> Enabling service (will auto-start on boot)..."
sudo systemctl enable homeautomation

echo "==> Setup complete."
