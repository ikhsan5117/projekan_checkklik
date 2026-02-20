@echo off
set "PROJECT_NAME=checklistMechine"
set "PUBLISH_DIR=checklistAM"
cd /d "%~dp0"

echo ==========================================
echo      Mempublikasikan %PROJECT_NAME%...
echo ==========================================

:: Hapus folder publish lama jika ada
if exist "%PUBLISH_DIR%" (
    echo Menghapus folder publish lama...
    rmdir /s /q "%PUBLISH_DIR%"
)

:: Jalankan dotnet publish
echo Sedang membuild dan publish project...
dotnet publish "AMRVI.csproj" -c Release -o "%PUBLISH_DIR%"

if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Gagal melakukan publish! Silakan cek error di atas.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo      Publish Selesai!
echo ==========================================
echo File hasil publish berada di folder: %CD%\%PUBLISH_DIR%
echo.
echo Langkah selanjutnya untuk deploy ke IIS:
echo 1. Pastikan .NET Core Hosting Bundle sudah terinstall di server IIS.
echo 2. Buat Website baru di IIS Manager, arahkan ke folder tempat Anda akan menaruh file ini.
echo 3. Stop Application Pool di IIS jika sedang berjalan (jika update).
echo 4. Copy semua file dari folder "%PUBLISH_DIR%" ke folder website IIS (misal: C:\inetpub\wwwroot\checklistMechine).
echo 5. Start kembali Application Pool.
echo.
pause
