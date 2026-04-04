# Home Energy Dashboard

A real-time energy monitoring dashboard for home solar systems. Displays battery state, solar input, inverter settings, and local weather to help make informed decisions about energy usage.

Built with ASP.NET Core 10 and Angular 21, designed to run on a Raspberry Pi.

![High level architecture](HomeAutomation.Web/ClientApp/src/assets/images/High-level%20architecture.png)

## Features

- **Battery** — state of charge, power in/out, estimated capacity
- **Inverter settings** — current operating mode and charge limits
- **Weather** — local forecast via [Open-Meteo](https://open-meteo.com/)
- **Cloud fallback** — falls back to Solax cloud API if the local inverter is unreachable

## Tech stack

- [ASP.NET Core 10](https://get.asp.net/) + C# — backend API
- [Angular 21](https://angular.io/) + TypeScript — frontend
- [Bootstrap](http://getbootstrap.com/) — layout and styling
- [MediatR](https://github.com/jbogard/MediatR) — CQRS request handling
- [Open-Meteo](https://open-meteo.com/) — free weather API (no key required)
- [Solax](https://www.solaxcloud.com/) — local and cloud inverter API

---

## Running locally

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)

### 1. Clone and restore

```bash
git clone https://github.com/emma-cogensoft/HomeAutomation.git
cd HomeAutomation
dotnet restore
cd HomeAutomation.Web/ClientApp && npm install && cd ../..
```

### 2. Configure secrets

Copy the example config and set your secrets using .NET user secrets:

```bash
cd HomeAutomation.Web

# Local inverter (LAN access)
dotnet user-secrets set "Services:LocalInverterApiSettingsOptions:RequestUri" "http://YOUR_INVERTER_LAN_IP"
dotnet user-secrets set "Services:LocalInverterApiSettingsOptions:RequestBody" "optType={REQUESTNAME}&pwd=YOUR_INVERTER_PASSWORD"

# Solax cloud API (fallback when local is unreachable)
dotnet user-secrets set "Services:CloudInverterApiSettingsOptions:RequestTemplateUri" "https://www.solaxcloud.com/proxyApp/proxy/api/{REQUESTNAME}.do?tokenId=YOUR_TOKEN&sn=YOUR_SERIAL"

# Octopus Energy (optional — not yet wired up)
dotnet user-secrets set "Services:OctopusEnergyApiOptions:RequestUri" "https://api.octopus.energy/v1/products/YOUR_PRODUCT/electricity-tariffs/YOUR_TARIFF"
```

Update your location in `HomeAutomation.Web/appsettings.json`:

```json
"OpenMeteoApiSettingsOptions": {
  "Latitude": 51.5,
  "Longitude": -0.1
}
```

See `HomeAutomation.Web/appsettings.example.json` for a full reference of all config keys.

### 3. Run

```bash
cd HomeAutomation.Web
dotnet run
```

The Angular dev server starts automatically. Open **https://localhost:44404** in your browser.

> **Note:** the dev server proxies API calls from port 44404 → port 5165 (the .NET backend). If you see CORS errors, make sure you're using https://localhost:44404, not 7181.

---

## Deploying to Raspberry Pi

The app runs on a single Raspberry Pi 4 (or Pi 3B+). The Angular SPA is compiled into static files and served directly by ASP.NET Core — no separate Node.js server or reverse proxy needed. A browser in kiosk mode on the same Pi displays the dashboard full-screen via HDMI.

### Pi setup (first time)

1. Flash **Raspberry Pi OS 64-bit (Trixie or Bookworm)** using Raspberry Pi Imager — configure SSH, WiFi, and a hostname (e.g. `homeautomation`) in the advanced settings
2. Boot the Pi and SSH in: `ssh YOUR_USER@homeautomation.local`
3. Update the OS: `sudo apt update && sudo apt upgrade -y`
4. Enable desktop auto-login: `sudo raspi-config` → System Options → Boot / Auto Login → Desktop Autologin
5. Set up SSH key authentication to avoid password prompts on redeploy: `ssh-copy-id YOUR_USER@homeautomation.local`

### Automated deploy (recommended)

The `scripts/deploy.ps1` script handles the full deployment from your Windows machine — build, stop service, copy files, service setup, secrets, and health check.

```powershell
.\scripts\deploy.ps1 -PiHost homeautomation.local -PiUser YOUR_PI_USERNAME
```

The script will:
1. Build a self-contained linux-arm64 binary (Angular SPA included)
2. Stop the running service so the binary isn't locked
3. Copy everything to `/opt/homeautomation/` on the Pi
4. Create and enable a systemd service (via `scripts/setup.sh`)
5. Install Epiphany browser and emoji font support
6. Configure kiosk autostart (XDG `.desktop` entry)
7. Read your `.NET user-secrets` and write them to the Pi's systemd override file
8. Restart the service and verify with a health check

> **Re-deploying?** Just run the same command again — it's fully idempotent.

### Kiosk display

The deploy script configures **Epiphany (GNOME Web)** to launch automatically on boot pointing at `http://127.0.0.1:5000`. Press **F11** to go full-screen.

The browser launches 20 seconds after desktop login to give the .NET service time to start.

> **Note:** Chromium also works but shows a low-RAM warning on Pi 3 devices (< 1GB RAM). Epiphany is lighter and warning-free.

### Manual deploy

<details>
<summary>Expand for manual steps</summary>

#### Build for Pi (ARM64)

On your development machine:

```bash
cd HomeAutomation.Web
dotnet publish -r linux-arm64 --self-contained -c Release -o ./publish
```

This produces a self-contained binary — no .NET runtime needed on the Pi.

#### Stop the service and copy to Pi

```bash
ssh YOUR_USER@homeautomation.local "sudo systemctl stop homeautomation"
scp -r ./publish/. YOUR_USER@homeautomation.local:/opt/homeautomation/
```

#### Configure secrets on Pi

Set secrets as environment variables. ASP.NET Core maps `Services__X__Y` → `Services:X:Y`:

```bash
sudo systemctl edit homeautomation --force
```

Add under `[Service]`:

```ini
Environment="ASPNETCORE_URLS=http://+:5000"
Environment="Services__LocalInverterApiSettingsOptions__RequestUri=http://YOUR_INVERTER_LAN_IP"
Environment="Services__LocalInverterApiSettingsOptions__RequestBody=optType={REQUESTNAME}&pwd=YOUR_PASSWORD"
Environment="Services__CloudInverterApiSettingsOptions__RequestTemplateUri=https://www.solaxcloud.com/proxyApp/proxy/api/{REQUESTNAME}.do?tokenId=YOUR_TOKEN&sn=YOUR_SERIAL"
Environment="Services__OpenMeteoApiSettingsOptions__Latitude=51.5"
Environment="Services__OpenMeteoApiSettingsOptions__Longitude=-0.1"
```

#### Create systemd service on Pi

Create `/etc/systemd/system/homeautomation.service`:

```ini
[Unit]
Description=Home Energy Dashboard
After=network.target

[Service]
Type=simple
User=YOUR_USER
WorkingDirectory=/opt/homeautomation
ExecStart=/opt/homeautomation/HomeAutomation.Web
Restart=on-failure
RestartSec=10

[Install]
WantedBy=multi-user.target
```

Enable and start:

```bash
sudo systemctl daemon-reload
sudo systemctl enable homeautomation
sudo systemctl start homeautomation
```

Check it's running:

```bash
curl http://localhost:5000/api/healthcheck
# Expected: 1
```

#### Configure kiosk autostart

```bash
sudo apt install -y epiphany-browser fonts-noto-color-emoji
mkdir -p ~/.config/autostart
cat > ~/.config/autostart/homeautomation-kiosk.desktop << 'EOF'
[Desktop Entry]
Type=Application
Name=Home Automation Kiosk
Exec=bash -c "sleep 20 && epiphany --application-mode --profile=/tmp/ha-kiosk http://127.0.0.1:5000"
X-GNOME-Autostart-enabled=true
EOF
```

Press **F11** in Epiphany to go full-screen.

</details>

---

## Resource requirements

| | Minimum |
|--|--|
| RAM | 1 GB (Pi 3B+ or Pi 4 recommended) |
| Disk | ~100 MB for the published app |
| Network | LAN access to inverter; internet for cloud fallback + weather |

---

## Troubleshooting

**Battery shows "Loading..."**
- Check the backend is running: `curl http://localhost:5000/api/healthcheck`
- Check local inverter is reachable: `curl http://YOUR_INVERTER_LAN_IP`
- If local is down, cloud fallback activates automatically

**Weather shows "Loading..."**
- Open-Meteo is a public API — check internet connectivity
- Verify latitude/longitude in `appsettings.json`

**Inverter settings shows "Loading..." / 503**
- Inverter settings are local-only (no cloud fallback)
- The local inverter must be reachable on the LAN

**App won't start**
- Check logs: `sudo journalctl -u homeautomation -n 50`
- Verify environment variables are set correctly: `sudo systemctl show homeautomation`

**Kiosk browser doesn't launch on boot**
- Check auto-login is enabled: `sudo raspi-config` → System Options → Boot / Auto Login
- Check the autostart file: `cat ~/.config/autostart/homeautomation-kiosk.desktop`
- Try launching manually: `epiphany --application-mode --profile=/tmp/ha-kiosk http://127.0.0.1:5000`
