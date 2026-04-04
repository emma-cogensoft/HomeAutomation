#!/bin/bash
# Runs on the Raspberry Pi to configure the systemd service and kiosk mode.
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

echo "==> Configuring kiosk mode..."
AUTOSTART_DIR="/home/${PI_USER}/.config/autostart"
mkdir -p "$AUTOSTART_DIR"

# XDG autostart entry — works with GNOME and other Wayland compositors on Trixie
tee "$AUTOSTART_DIR/homeautomation-kiosk.desktop" > /dev/null <<EOF
[Desktop Entry]
Type=Application
Name=Home Automation Kiosk
Exec=chromium-browser --kiosk --noerrdialogs --disable-infobars --disable-session-crashed-bubble --app=http://localhost:5000
X-GNOME-Autostart-enabled=true
EOF

# Disable screensaver and screen blanking via GNOME settings (Trixie default)
# These are set as the Pi user so they apply to the desktop session
sudo -u "$PI_USER" gsettings set org.gnome.desktop.screensaver lock-enabled false 2>/dev/null || true
sudo -u "$PI_USER" gsettings set org.gnome.desktop.session idle-delay 0 2>/dev/null || true

echo "==> NOTE: Ensure auto-login is enabled in raspi-config:"
echo "    sudo raspi-config -> System Options -> Boot / Auto Login -> Desktop Autologin"
echo ""
echo "==> Setup complete."
