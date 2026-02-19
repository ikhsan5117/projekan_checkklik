@echo off
setlocal
pushd "%~dp0"

:: ==========================================
:: KONFIGURASI
:: ==========================================
set "DEST_PATH=D:\WebApps\DailyCheckMechine"
set "TEMP_DIR=___UPDATE_TEMP___"
:: ==========================================

cls
echo ============================================================
echo      PROSES UPDATE KE DRIVE D: (DEBUG MODE)
echo ============================================================
echo.

:: 0. Pembersihan Folder Sisa
echo [Langkah 0] Membersihkan sisa-sisa update lama...
if exist "DailyCheckMechine_Update" rd /s /q "DailyCheckMechine_Update"
if exist "DailyCheckMechine_TEMP" rd /s /q "DailyCheckMechine_TEMP"
if exist "%TEMP_DIR%" rd /s /q "%TEMP_DIR%"
echo Pembersihan selesai.
echo.

:: 1. Jalankan Publish
echo [Langkah 1] Membangun paket aplikasi (.NET Publish)...
call dotnet publish -c Release -o "%TEMP_DIR%" --no-restore
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [INFO] Mencoba build ulang dengan restore otomatis...
    call dotnet publish -c Release -o "%TEMP_DIR%"
)

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Build Gagal! Cek apakah ada error di kode program.
    goto :FAILED_PROCESS
)
echo [OK] Paket aplikasi berhasil dibuat di folder %TEMP_DIR%.
echo Silakan tekan tombol apa saja untuk lanjut ke Step 2...
pause

:: 2. Masuk ke mode Offline
echo.
echo [Langkah 2] Menidurkan aplikasi di server (Maintenance)...
if not exist "%DEST_PATH%" (
    echo [ERROR] Folder TUJUAN tidak ditemukan: %DEST_PATH%
    goto :FAILED_PROCESS
)

:: Membuat file offline
echo Update sedang berlangsung... > "%DEST_PATH%\app_offline.htm"
if errorlevel 1 (
    echo [ERROR] Gagal menulis ke Drive D. Cek izin akses folder!
    goto :FAILED_PROCESS
)
echo [OK] Aplikasi sudah dalam mode maintenance.
timeout /t 3 /nobreak

:: 3. Jalankan Sinkronisasi File
echo.
echo [Langkah 3] Menyalin file baru ke folder IIS...
:: Perintah Robocopy: /E (Subfolder), /XO (Hanya yang baru), /XF (Kecuali DB dan Config)
robocopy "%TEMP_DIR%" "%DEST_PATH%" /E /R:3 /W:2 /MT:8 /XF appsettings.json appsettings.Development.json app.db

:: Robocopy exit code 8 ke atas artinya ERROR
if %ERRORLEVEL% GEQ 8 (
    echo.
    echo [ERROR] Gagal menyalin file. Kemungkinan ada file yang sedang dibuka atau izin ditolak.
    goto :FAILED_PROCESS
)
echo [OK] File berhasil diperbarui.
echo Silakan tekan tombol apa saja untuk lanjut ke Step 4...
pause

:: 4. Hidupkan Kembali
echo.
echo [Langkah 4] Menghidupkan kembali aplikasi...
if exist "%DEST_PATH%\app_offline.htm" del "%DEST_PATH%\app_offline.htm"
echo [OK] Aplikasi sudah aktif kembali.

:: Bersihkan folder temp
rd /s /q "%TEMP_DIR%"

echo.
echo ============================================================
echo   BERHASIL! Update Dashboard Henkaten sudah terkirim.
echo ============================================================
goto :END_PROCESS

:FAILED_PROCESS
echo.
echo [MAAF] Ada kendala pada proses di atas. 
echo Mohon jangan ditutup, silakan foto/screenshot pesan error di atas.
pause

:END_PROCESS
echo.
echo Selesai. Tekan tombol apa saja untuk tutup.
pause
popd
