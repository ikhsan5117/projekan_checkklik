# Script publish Daily_Checklist_Machine untuk deploy ke IIS
# Jalankan: .\publish-for-iis.ps1
# Hasil: folder "Daily_Checklist_Machine" siap dipindah ke server IIS

$ErrorActionPreference = "Stop"
$projectRoot = $PSScriptRoot
$publishFolder = "Daily_Checklist_Machine"

Set-Location $projectRoot

Write-Host "Publish Daily_Checklist_Machine untuk IIS..." -ForegroundColor Cyan
dotnet publish -c Release -o ".\$publishFolder"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Publish gagal." -ForegroundColor Red
    exit 1
}

# Salin panduan singkat ke dalam folder publish
if (Test-Path ".\Baca_Saya_IIS.txt") {
    Copy-Item ".\Baca_Saya_IIS.txt" -Destination ".\$publishFolder\Baca_Saya_IIS.txt" -Force
}

Write-Host ""
Write-Host "Selesai. Folder 'Daily_Checklist_Machine' siap dipindah ke IIS." -ForegroundColor Green
Write-Host "Isi folder sudah termasuk file Baca_Saya_IIS.txt (panduan singkat)." -ForegroundColor Gray
Write-Host ""

# Opsi: buka folder publish
$buka = Read-Host "Buka folder publish sekarang? (Y/n)"
if ($buka -ne "n" -and $buka -ne "N") {
    explorer.exe (Resolve-Path ".\$publishFolder")
}
