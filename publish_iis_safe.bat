@echo off
REM ==========================================
REM Script Publish untuk IIS - Versi Aman
REM ==========================================
REM Script ini TIDAK akan menghapus dirinya sendiri
REM ==========================================

setlocal enabledelayedexpansion
set "PROJECT_NAME=AMRVI"
set "PUBLISH_DIR=publish"
set "SCRIPT_DIR=%~dp0"

REM Pindah ke direktori script
cd /d "%SCRIPT_DIR%"

echo ==========================================
echo      Mempublikasikan %PROJECT_NAME%...
echo ==========================================
echo.
echo Script ini akan:
echo 1. Menghapus folder publish lama (jika ada)
echo 2. Membuild dan publish project ke folder "%PUBLISH_DIR%"
echo.
echo Tekan Ctrl+C untuk membatalkan, atau
pause

REM Hapus folder publish lama jika ada
if exist "%PUBLISH_DIR%" (
    echo.
    echo [INFO] Menghapus folder publish lama...
    rmdir /s /q "%PUBLISH_DIR%"
    if !ERRORLEVEL! EQU 0 (
        echo [OK] Folder lama berhasil dihapus.
    ) else (
        echo [WARNING] Gagal menghapus folder lama, lanjutkan...
    )
)

REM Jalankan dotnet publish
echo.
echo [INFO] Sedang membuild dan publish project...
echo.
dotnet publish "AMRVI.csproj" -c Release -o "%PUBLISH_DIR%"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Gagal melakukan publish! Silakan cek error di atas.
    echo.
    pause
    exit /b %ERRORLEVEL%
)

echo.
echo ==========================================
echo      Publish Selesai!
echo ==========================================
echo.
echo File hasil publish berada di folder:
echo %CD%\%PUBLISH_DIR%
echo.
echo ==========================================
echo Langkah selanjutnya untuk deploy ke IIS:
echo ==========================================
echo 1. Pastikan .NET Core Hosting Bundle sudah terinstall di server IIS.
echo 2. Buat Website baru di IIS Manager, arahkan ke folder tempat Anda akan menaruh file ini.
echo 3. Stop Application Pool di IIS jika sedang berjalan (jika update).
echo 4. Copy semua file dari folder "%PUBLISH_DIR%" ke folder website IIS.
echo    Contoh: C:\inetpub\wwwroot\%PROJECT_NAME%
echo 5. Start kembali Application Pool.
echo.
echo Tekan tombol apa saja untuk menutup...
pause >nul
