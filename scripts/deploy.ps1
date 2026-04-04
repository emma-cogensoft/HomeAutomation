<#
.SYNOPSIS
    Builds and deploys the Home Automation app to a Raspberry Pi.
.DESCRIPTION
    1. Builds a self-contained linux-arm64 binary
    2. Copies it to the Pi via scp
    3. Configures a systemd service
    4. Reads .NET user-secrets and writes them as environment variables
       in the systemd override file on the Pi (clearly displayed before writing)
    5. Restarts the service and verifies with a health check
.EXAMPLE
    .\deploy.ps1 -PiHost homeautomation.local -PiUser emma
#>
param(
    [Parameter(Mandatory)]
    [string]$PiHost,

    [Parameter(Mandatory)]
    [string]$PiUser,

    [string]$PiPath = "/opt/homeautomation"
)

$ErrorActionPreference = "Stop"

function Step($msg)    { Write-Host "`n==> $msg" -ForegroundColor Cyan }
function Success($msg) { Write-Host "    OK: $msg" -ForegroundColor Green }

# ── 1. Build ──────────────────────────────────────────────────────────────────
Step "Building for linux-arm64 (self-contained release)..."
$webDir = Join-Path $PSScriptRoot "..\HomeAutomation.Web"
Push-Location $webDir
try {
    dotnet publish -r linux-arm64 --self-contained -c Release -o ./publish
    if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed" }
} finally {
    Pop-Location
}
Success "Build complete."

# ── 2. Read secrets ───────────────────────────────────────────────────────────
Step "Reading .NET user-secrets..."
Push-Location $webDir
try {
    $secretsRaw = dotnet user-secrets list 2>&1
    if ($LASTEXITCODE -ne 0) { throw "dotnet user-secrets list failed. Run 'dotnet user-secrets list' in HomeAutomation.Web to diagnose." }
} finally {
    Pop-Location
}

$envLines = [System.Collections.Generic.List[string]]::new()
$envLines.Add('Environment="ASPNETCORE_URLS=http://+:5000"')
foreach ($line in $secretsRaw) {
    if ($line -match "^(.+?)\s*=\s*(.+)$") {
        $key   = $Matches[1].Trim().Replace(":", "__")
        $value = $Matches[2].Trim()
        $envLines.Add("Environment=`"${key}=${value}`"")
    }
}

Write-Host ""
Write-Host "    *** The following secrets will be read from your local user-secrets store ***" -ForegroundColor Yellow
Write-Host "    *** and written to the Pi at:                                              ***" -ForegroundColor Yellow
Write-Host "    *** /etc/systemd/system/homeautomation.service.d/override.conf             ***" -ForegroundColor Yellow
Write-Host ""
foreach ($line in $envLines) {
    Write-Host "      $line" -ForegroundColor DarkYellow
}
Write-Host ""

# ── 3. Prepare directory on Pi ────────────────────────────────────────────────
Step "Preparing $PiPath on Pi..."
ssh "${PiUser}@${PiHost}" "sudo mkdir -p $PiPath && sudo chown ${PiUser}:${PiUser} $PiPath"
Success "Directory ready."

# ── 4. Copy published files ───────────────────────────────────────────────────
Step "Copying published files to Pi..."
$publishDir = Join-Path $webDir "publish"
$publishSource = $publishDir.Replace('\', '/') + "/*"
scp -r $publishSource "${PiUser}@${PiHost}:${PiPath}/"
if ($LASTEXITCODE -ne 0) { throw "scp failed" }
Success "Files copied."

# ── 5. Run setup script on Pi ─────────────────────────────────────────────────
Step "Running setup script on Pi..."
$setupScript = Join-Path $PSScriptRoot "setup.sh"
scp $setupScript "${PiUser}@${PiHost}:/tmp/ha-setup.sh"
# Strip any Windows CRLF line endings before running
ssh "${PiUser}@${PiHost}" "sed -i 's/\r//' /tmp/ha-setup.sh && bash /tmp/ha-setup.sh '$PiPath' '${PiUser}' && rm /tmp/ha-setup.sh"
if ($LASTEXITCODE -ne 0) { throw "Setup script failed" }
Success "Service file created and enabled."

# ── 6. Write secrets to override.conf ─────────────────────────────────────────
Step "Writing secrets to systemd override.conf on Pi..."
$overrideDir  = "/etc/systemd/system/homeautomation.service.d"
$overridePath = "$overrideDir/override.conf"

$overrideContent = "[Service]`n" + ($envLines -join "`n")

# Pipe content to a temp file on the Pi, then sudo-move it into place
$overrideContent | ssh "${PiUser}@${PiHost}" "cat > /tmp/ha-override.conf"
ssh "${PiUser}@${PiHost}" "sudo mkdir -p $overrideDir && sudo mv /tmp/ha-override.conf $overridePath && sudo chmod 600 $overridePath"
if ($LASTEXITCODE -ne 0) { throw "Failed to write override.conf" }
Success "Secrets written to $overridePath."

# ── 7. Reload and restart ─────────────────────────────────────────────────────
Step "Reloading systemd and restarting service..."
ssh "${PiUser}@${PiHost}" "sudo systemctl daemon-reload && sudo systemctl restart homeautomation"
if ($LASTEXITCODE -ne 0) { throw "Service failed to start" }
Success "Service started."

# ── 8. Health check ───────────────────────────────────────────────────────────
Step "Running health check..."
Start-Sleep -Seconds 3
$health = ssh "${PiUser}@${PiHost}" "curl -sf http://localhost:5000/api/healthcheck 2>/dev/null"
if ($health -eq "1") {
    Write-Host "`n    Deployment successful!" -ForegroundColor Green
    Write-Host "    App is running at: http://${PiHost}:5000" -ForegroundColor Green
} else {
    Write-Host "`n    Health check failed. Check logs with:" -ForegroundColor Red
    Write-Host "    ssh ${PiUser}@${PiHost} 'sudo journalctl -u homeautomation -n 50'" -ForegroundColor Red
    exit 1
}
