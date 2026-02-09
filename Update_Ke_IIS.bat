@echo off
setlocal
pushd "%~dp0"

:: ==========================================
:: CONFIGURATION
:: ==========================================
set "SERVER_IP=10.14.173.54"
set "DEST_PATH=\\%SERVER_IP%\c$\inetpub\wwwroot\DailyChecklistMachine_Andon"
:: ==========================================

echo.
echo ============================================================
echo      FORCE UPDATE ANDON SYSTEM TO IIS (%SERVER_IP%)
echo ============================================================
echo.

:: 1. Build project
echo [1/4] Memproses Kode C# (Building)...
dotnet build -c Release /p:Optimize=true

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [ERROR] Gagal memproses kode.
    popd
    pause
    exit /b %ERRORLEVEL%
)

:: 2. Masuk ke mode Offline (PENTING)
echo [2/4] Menidurkan aplikasi di server...
echo ^<html^>^<body overflow:hidden;background:#0f172a;color:white;font-family:sans-serif^>^<div style="text-align:center;padding-top:20%%"^>^<h2 style="color:#3b82f6"^>System Update in Progress^</h2^>^<p^>Please wait for 10 seconds...^</p^>^</div^>^</body^>^</html^> > "%DEST_PATH%\app_offline.htm"

:: Menunggu 5 detik (Lebih lama agar IIS benar-benar melepaskan file)
echo Menunggu IIS melepaskan file (5 detik)...
timeout /t 5 /nobreak > nul

:: 3. Jalankan Update (Gunakan Robocopy untuk fitur Retry)
echo [3/4] Mengirim file terbaru (Force Overwrite)...

:: Kirim DLL & PDB (Gunakan Robocopy agar ada usaha retry jika masih terkunci sebentar)
robocopy "bin\Release\net8.0" "%DEST_PATH%" AMRVI.dll AMRVI.pdb /R:3 /W:2 /NJH /NJS

:: Update Tampilan & Static Files
robocopy "Views" "%DEST_PATH%\Views" /E /XO /NJH /NJS /NDL /NC /NS /R:3 /W:2
robocopy "wwwroot" "%DEST_PATH%\wwwroot" /E /XO /NJH /NJS /NDL /NC /NS /R:3 /W:2

:: 4. Hidupkan Kembali
echo [4/4] Membangunkan kembali aplikasi...
if exist "%DEST_PATH%\app_offline.htm" del "%DEST_PATH%\app_offline.htm"

echo.
echo ============================================================
echo   SELESAI! Silakan cek web di browser.
echo   (Jika masih duplikat, tekan Ctrl+F5 di browser)
echo ============================================================
echo.
popd
pause
