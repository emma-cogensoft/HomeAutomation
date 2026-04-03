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

The recommended setup is two Pis:

| Device | Role |
|--------|------|
| **Pi 4** | Runs the ASP.NET Core backend as a systemd service |
| **Pi 3** | Kiosk display — runs Chromium pointed at the Pi 4 |

### Build for Pi 4 (ARM64)

On your development machine:

```bash
cd HomeAutomation.Web
dotnet publish -r linux-arm64 --self-contained -c Release -o ./publish
```

This produces a self-contained binary — no .NET runtime needed on the Pi.

### Copy to Pi 4

```bash
scp -r ./publish/* pi@YOUR_PI4_IP:/opt/homeautomation/
```

### Configure secrets on Pi 4

Set secrets as environment variables. ASP.NET Core maps `Services__X__Y` → `Services:X:Y`:

```bash
sudo systemctl edit homeautomation --force
```

Add under `[Service]`:

```ini
Environment="Services__LocalInverterApiSettingsOptions__RequestUri=http://YOUR_INVERTER_LAN_IP"
Environment="Services__LocalInverterApiSettingsOptions__RequestBody=optType={REQUESTNAME}&pwd=YOUR_PASSWORD"
Environment="Services__CloudInverterApiSettingsOptions__RequestTemplateUri=https://www.solaxcloud.com/proxyApp/proxy/api/{REQUESTNAME}.do?tokenId=YOUR_TOKEN&sn=YOUR_SERIAL"
Environment="Services__OpenMeteoApiSettingsOptions__Latitude=51.5"
Environment="Services__OpenMeteoApiSettingsOptions__Longitude=-0.1"
Environment="Services__BatteryOptions__CapacityInWh=11600"
```

### Create systemd service on Pi 4

Create `/etc/systemd/system/homeautomation.service`:

```ini
[Unit]
Description=Home Energy Dashboard
After=network.target

[Service]
Type=simple
User=pi
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
curl http://localhost:5165/health
# Expected: Healthy
```

### Set up Pi 3 as a kiosk display

See [issue #34](https://github.com/emma-cogensoft/HomeAutomation/issues/34) for full kiosk setup. In short — Chromium in kiosk mode pointing at `http://YOUR_PI4_LAN_IP:5165`.

---

## Resource requirements

| | Minimum |
|--|--|
| RAM | 512 MB (Pi 4 recommended for comfortable headroom) |
| Disk | ~100 MB for the published app |
| Network | LAN access to inverter; internet for cloud fallback + weather |

---

## Troubleshooting

**Battery shows "Loading..."**
- Check the backend is running: `curl http://localhost:5165/health`
- Check local inverter is reachable: `curl http://YOUR_INVERTER_LAN_IP`
- If local is down, cloud fallback activates automatically (yellow banner shown)

**Weather shows "Loading..."**
- Open-Meteo is a public API — check internet connectivity
- Verify latitude/longitude in `appsettings.json`

**Inverter settings shows "Loading..." / 503**
- Inverter settings are local-only (no cloud fallback)
- The local inverter must be reachable on the LAN

**App won't start**
- Check logs: `sudo journalctl -u homeautomation -n 50`
- Verify environment variables are set correctly: `sudo systemctl show homeautomation`
