@echo off
REM =====================================================
REM SCRIPT VERIFICA INSTALLAZIONE - CONSULTING GROUP SRL
REM =====================================================

echo.
echo ============================================
echo VERIFICA INSTALLAZIONE - CONSULTING GROUP SRL
echo ============================================
echo.

echo [VERIFICA PREREQUISITI]
echo.

REM Verifica .NET Runtime
echo Verificando .NET Runtime...
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App 9." >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ .NET 9.0 ASP.NET Core Runtime installato
) else (
    echo ✗ .NET 9.0 ASP.NET Core Runtime NON installato
    echo   Scaricare da: https://dotnet.microsoft.com/download/dotnet/9.0
)

REM Verifica IIS
echo Verificando IIS...
sc query W3SVC >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ IIS installato e in esecuzione
) else (
    echo ✗ IIS non installato o non in esecuzione
)

echo.
echo [VERIFICA FILES APPLICAZIONE]
echo.

REM Verifica cartelle
if exist "C:\inetpub\consulting.web" (
    echo ✓ Cartella applicazione presente
) else (
    echo ✗ Cartella applicazione mancante
)

REM Verifica file principali
if exist "C:\inetpub\consulting.web\ConsultingGroup.dll" (
    echo ✓ Assembly principale presente
) else (
    echo ✗ Assembly principale mancante
)

if exist "C:\inetpub\consulting.web\web.config" (
    echo ✓ web.config presente
) else (
    echo ✗ web.config mancante
)

if exist "C:\inetpub\consulting.web\appsettings.Production.json" (
    echo ✓ Configurazione produzione presente
) else (
    echo ✗ Configurazione produzione mancante
)

REM Verifica Views
if exist "C:\inetpub\consulting.web\Views" (
    echo ✓ Cartella Views presente
    
    REM Conta file views
    for /f %%i in ('dir /s /b "C:\inetpub\consulting.web\Views\*.cshtml" 2^>nul ^| find /c ".cshtml"') do set viewcount=%%i
    echo   Numero Views trovate: !viewcount!
    
    if !viewcount! GEQ 60 (
        echo ✓ Views complete
    ) else (
        echo ⚠ Possibili Views mancanti
    )
) else (
    echo ✗ Cartella Views mancante
)

echo.
echo [VERIFICA CONFIGURAZIONE IIS]
echo.

REM Verifica sito IIS
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ Sito IIS configurato
    %windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup"
) else (
    echo ✗ Sito IIS non configurato
)

REM Verifica pool applicazioni
%windir%\system32\inetsrv\appcmd.exe list apppool "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ Pool applicazioni configurato
) else (
    echo ✗ Pool applicazioni non configurato
)

echo.
echo [VERIFICA PERMESSI]
echo.

REM Verifica permessi cartella
icacls "C:\inetpub\consulting.web" | findstr "IIS_IUSRS" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ Permessi IIS_IUSRS configurati
) else (
    echo ✗ Permessi IIS_IUSRS mancanti
)

echo.
echo [TEST CONNETTIVITÀ]
echo.

REM Test HTTP (se il sito è in esecuzione)
echo Testando connessione HTTP...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost' -UseBasicParsing -TimeoutSec 10; if ($response.StatusCode -eq 200) { Write-Host '✓ Sito web raggiungibile' } else { Write-Host '⚠ Sito web risponde ma con errori' } } catch { Write-Host '✗ Sito web non raggiungibile' }"

echo.
echo ============================================
echo VERIFICA COMPLETATA
echo ============================================
echo.
echo Per accedere al sistema:
echo 1. Aprire browser: http://localhost
echo 2. Login amministratore: admin / 123456
echo 3. Login senior: senior / 123456
echo 4. Login utente: user / 123456
echo.

pause
