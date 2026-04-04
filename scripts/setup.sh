#!/bin/bash
# Runs on the Raspberry Pi to configure the systemd service and kiosk mode.
# Called automatically by deploy.ps1, but can also be run manually:
#   bash setup.sh [pi-path] [pi-user]
set -e

PI_PATH="${1:-/opt/homeautomation}"
PI_USER="${2:-pi}"

echo "==> Installing browser and emoji font support..."
sudo apt install -y epiphany-browser fonts-noto-color-emoji
fc-cache -fv > /dev/null

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
sudo -u "$PI_USER" mkdir -p "$AUTOSTART_DIR"

# XDG autostart entry — works with GNOME and other Wayland compositors on Trixie
sudo -u "$PI_USER" tee "$AUTOSTART_DIR/homeautomation-kiosk.desktop" > /dev/null <<EOF
[Desktop Entry]
Type=Application
Name=Home Automation Kiosk
Exec=bash -c "sleep 20 && epiphany --application-mode --profile=/tmp/ha-kiosk http://127.0.0.1:5000"
X-GNOME-Autostart-enabled=true
EOF

# Disable screensaver and screen blanking via GNOME settings.
# Run inside a temporary DBus session so this works non-interactively over SSH.
if command -v dbus-run-session >/dev/null 2>&1; then
    sudo -u "$PI_USER" env HOME="/home/${PI_USER}" dbus-run-session -- sh -c '
        gsettings set org.gnome.desktop.screensaver lock-enabled false &&
        gsettings set org.gnome.desktop.session idle-delay 0
    ' || echo "==> WARNING: Failed to apply GNOME idle/blanking settings. Apply them from a logged-in desktop session if needed."
else
    echo "==> WARNING: dbus-run-session not available; GNOME idle/blanking settings were not applied."
fi

echo "==> NOTE: Ensure auto-login is enabled in raspi-config:"
echo "    sudo raspi-config -> System Options -> Boot / Auto Login -> Desktop Autologin"
echo ""
echo "==> Setup complete."
