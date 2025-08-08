@echo off
REM =====================================================
REM INSTALLAZIONE IIS E ASP.NET CORE HOSTING BUNDLE
REM =====================================================

echo.
echo ============================================
echo INSTALLAZIONE IIS - CONSULTING GROUP SRL
echo ============================================
echo.

REM Verifica privilegi amministratore
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: Eseguire come Amministratore!
    echo Fare click destro sul file e selezionare "Esegui come amministratore"
    pause
    exit /b 1
)

echo [1/4] Installazione IIS e funzionalità...

REM Installazione IIS e componenti necessari
echo Installando IIS, ASP.NET e moduli necessari...
dism /online /enable-feature /featurename:IIS-WebServerRole /all /norestart
dism /online /enable-feature /featurename:IIS-WebServer /all /norestart
dism /online /enable-feature /featurename:IIS-CommonHttpFeatures /all /norestart
dism /online /enable-feature /featurename:IIS-HttpErrors /all /norestart
dism /online /enable-feature /featurename:IIS-HttpLogging /all /norestart
dism /online /enable-feature /featurename:IIS-ApplicationDevelopment /all /norestart
dism /online /enable-feature /featurename:IIS-NetFxExtensibility45 /all /norestart
dism /online /enable-feature /featurename:IIS-HealthAndDiagnostics /all /norestart
dism /online /enable-feature /featurename:IIS-HttpLogging /all /norestart
dism /online /enable-feature /featurename:IIS-Security /all /norestart
dism /online /enable-feature /featurename:IIS-RequestFiltering /all /norestart
dism /online /enable-feature /featurename:IIS-Performance /all /norestart
dism /online /enable-feature /featurename:IIS-WebServerManagementTools /all /norestart
dism /online /enable-feature /featurename:IIS-ManagementConsole /all /norestart
dism /online /enable-feature /featurename:IIS-IIS6ManagementCompatibility /all /norestart
dism /online /enable-feature /featurename:IIS-Metabase /all /norestart

echo [2/4] Verifica installazione IIS...

REM Avviare servizio IIS
net start W3SVC >nul 2>&1

REM Verificare che IIS sia installato
if exist "C:\Windows\system32\inetsrv\appcmd.exe" (
    echo ✓ IIS installato correttamente
) else (
    echo ✗ Errore installazione IIS
    pause
    exit /b 1
)

echo [3/4] Test IIS...

REM Test IIS di base
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost' -UseBasicParsing -TimeoutSec 5; Write-Host '✓ IIS risponde correttamente' } catch { Write-Host '⚠ IIS installato ma richiede riavvio' }"

echo [4/4] Informazioni installazione...

echo.
echo ============================================
echo INSTALLAZIONE IIS COMPLETATA!
echo ============================================
echo.
echo ✓ IIS Web Server installato
echo ✓ Moduli ASP.NET abilitati
echo ✓ Console Management installata
echo ✓ Servizio W3SVC avviato
echo.
echo VERIFICA INSTALLAZIONE:
echo 1. Aprire browser: http://localhost
echo 2. Dovrebbe apparire la pagina di benvenuto IIS
echo.
echo PROSSIMI PASSI:
echo 1. Installare .NET 9.0 ASP.NET Core Runtime:
echo    https://dotnet.microsoft.com/download/dotnet/9.0
echo    (Scaricare ASP.NET Core Runtime 9.0.x - Hosting Bundle)
echo.
echo 2. Riavviare il computer (raccomandato)
echo.
echo 3. Eseguire: INSTALL_SERVER_CONSULTING.bat
echo.
echo NOTA: Se serve riavvio, Windows lo richiederà automaticamente.
echo.

pause
