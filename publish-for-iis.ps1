# Script publish DailyChecklistMachine_Andon untuk deploy ke IIS
# Jalankan: .\publish-for-iis.ps1
# Hasil: folder "DailyChecklistMachine_Andon" siap dipindah ke server IIS

$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$publishFolder = "DailyChecklistMachine_Andon"

Set-Location $projectRoot

# Pastikan path publish absolut agar dotnet publish konsisten
$publishPath = Join-Path -Path $projectRoot -ChildPath $publishFolder

Write-Host "Publish DailyChecklistMachine_Andon untuk IIS..." -ForegroundColor Cyan
dotnet publish -c Release -o $publishPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish gagal." -ForegroundColor Red
    exit 1
}

# Salin panduan singkat ke dalam folder publish (jika ada)
if (Test-Path (Join-Path $projectRoot "Baca_Saya_IIS.txt")) {
    Copy-Item (Join-Path $projectRoot "Baca_Saya_IIS.txt") -Destination (Join-Path $publishPath "Baca_Saya_IIS.txt") -Force
    Write-Host "File Baca_Saya_IIS.txt disalin ke folder publish." -ForegroundColor Gray
}

Write-Host ""
Write-Host "Selesai. Folder 'DailyChecklistMachine_Andon' siap dipindah ke IIS." -ForegroundColor Green
Write-Host "Lokasi: $publishPath" -ForegroundColor Gray
Write-Host ""

# Opsi: buka folder publish
$buka = Read-Host "Buka folder publish sekarang? (Y/n)"
if ($buka -ne "n" -and $buka -ne "N") {
    explorer.exe $publishPath
}
