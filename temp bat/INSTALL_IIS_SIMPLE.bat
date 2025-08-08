@echo off
REM =====================================================
REM INSTALLAZIONE IIS SEMPLIFICATA
REM =====================================================

echo.
echo ============================================
echo INSTALLAZIONE IIS SEMPLIFICATA
echo ============================================
echo.

REM Verifica privilegi amministratore
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: Eseguire come Amministratore!
    pause
    exit /b 1
)

echo [1/3] Installazione IIS base...

REM Installazione IIS di base - comandi essenziali
dism /online /enable-feature /featurename:IIS-WebServerRole /all /norestart
if %errorLevel% neq 0 (
    echo Errore installazione IIS base
    pause
    exit /b 1
)

echo [2/3] Installazione moduli ASP.NET...

REM Moduli per ASP.NET Core
dism /online /enable-feature /featurename:IIS-ApplicationDevelopment /all /norestart
dism /online /enable-feature /featurename:IIS-NetFxExtensibility45 /all /norestart

echo [3/3] Avvio servizi...

REM Avviare IIS
net start W3SVC >nul 2>&1

echo.
echo ============================================
echo INSTALLAZIONE COMPLETATA!
echo ============================================
echo.

REM Verifica installazione
if exist "C:\Windows\system32\inetsrv\appcmd.exe" (
    echo ✓ IIS installato correttamente
    echo ✓ appcmd.exe disponibile
) else (
    echo ⚠ IIS potrebbe richiedere riavvio
)

echo.
echo VERIFICA: Aprire browser su http://localhost
echo Dovrebbe apparire la pagina di benvenuto IIS
echo.
echo PROSSIMI PASSI:
echo 1. Scaricare .NET 9.0 Hosting Bundle
echo 2. Riavviare PC se richiesto
echo 3. Eseguire INSTALL_SERVER_CONSULTING.bat
echo.

pause
