@echo off
REM =====================================================
REM VERIFICA INSTALLAZIONE SERVER - CONSULTING GROUP SRL
REM Database: SRV-dc\SQLEXPRESS - Consulting - Windows Auth
REM URL: http://consulting.local
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
    for /f "tokens=*" %%i in ('dotnet --list-runtimes ^| findstr "Microsoft.AspNetCore.App 9."') do echo   %%i
) else (
    echo ✗ .NET 9.0 ASP.NET Core Runtime NON installato
)

REM Verifica IIS
echo.
echo Verificando IIS...
sc query W3SVC >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ IIS installato e in esecuzione
) else (
    echo ✗ IIS non installato o non in esecuzione
)

echo.
echo [VERIFICA DATABASE]
echo.

REM Verifica connessione SQL Server
echo Verificando connessione a SRV-dc\SQLEXPRESS...
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "SELECT @@SERVERNAME as Server, @@VERSION as Version" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ Connessione a SRV-dc\SQLEXPRESS riuscita
    echo   Server info:
    for /f "skip=2 tokens=*" %%i in ('sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "SELECT @@SERVERNAME as Server" -h -1') do echo   Server: %%i
) else (
    echo ✗ Impossibile connettersi a SRV-dc\SQLEXPRESS
    echo   Verificare che SQL Server Express sia in esecuzione
)

REM Verifica database Consulting
echo.
echo Verificando database Consulting...
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "SELECT name FROM sys.databases WHERE name = 'Consulting'" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ Database Consulting presente
    
    REM Verifica tabelle principali
    echo   Verificando tabelle...
    sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT COUNT(*) as TablesCount FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1 >nul 2>&1
    if %errorLevel% equ 0 (
        for /f "skip=2 tokens=*" %%i in ('sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE_TABLE'" -h -1') do (
            if %%i GTR 10 (
                echo   ✓ Tabelle create (%%i tabelle)
            ) else (
                echo   ⚠ Poche tabelle trovate (%%i)
            )
        )
    )
    
    REM Verifica utenti
    echo   Verificando utenti...
    sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT COUNT(*) FROM AspNetUsers" -h -1 >nul 2>&1
    if %errorLevel% equ 0 (
        for /f "skip=2 tokens=*" %%i in ('sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT COUNT(*) FROM AspNetUsers" -h -1') do (
            if %%i GEQ 3 (
                echo   ✓ Utenti predefiniti presenti (%%i utenti)
            ) else (
                echo   ⚠ Utenti predefiniti potrebbero mancare (%%i utenti)
            )
        )
    ) else (
        echo   ⚠ Tabella AspNetUsers non trovata
    )
) else (
    echo ✗ Database Consulting non trovato
)

echo.
echo [VERIFICA FILES APPLICAZIONE]
echo.

REM Verifica cartella applicazione
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
    echo   Verificando stringa connessione...
    findstr "SRV-dc" "C:\inetpub\consulting.web\appsettings.Production.json" >nul 2>&1
    if %errorLevel% equ 0 (
        echo   ✓ Stringa connessione SRV-dc configurata
    ) else (
        echo   ⚠ Stringa connessione SRV-dc non trovata
    )
) else (
    echo ✗ Configurazione produzione mancante
)

REM Verifica Views
if exist "C:\inetpub\consulting.web\Views" (
    echo ✓ Cartella Views presente
    
    for /f %%i in ('dir /s /b "C:\inetpub\consulting.web\Views\*.cshtml" 2^>nul ^| find /c ".cshtml"') do (
        echo   Views trovate: %%i
        if %%i GEQ 60 (
            echo   ✓ Views complete
        ) else (
            echo   ⚠ Possibili Views mancanti
        )
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
    echo   Configurazione:
    %windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup"
    
    REM Verifica binding consulting.local
    %windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" | findstr "consulting.local" >nul 2>&1
    if %errorLevel% equ 0 (
        echo   ✓ Binding consulting.local configurato
    ) else (
        echo   ⚠ Binding consulting.local non trovato
    )
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
echo [VERIFICA FILE HOSTS]
echo.

findstr "consulting.local" %windir%\System32\drivers\etc\hosts >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ consulting.local configurato in file hosts
    findstr "consulting.local" %windir%\System32\drivers\etc\hosts
) else (
    echo ✗ consulting.local non trovato in file hosts
)

echo.
echo [TEST CONNETTIVITÀ]
echo.

REM Test ping consulting.local
echo Testando risoluzione DNS consulting.local...
ping consulting.local -n 1 >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ consulting.local risolve correttamente
) else (
    echo ✗ consulting.local non risolve
)

REM Test HTTP
echo.
echo Testando connessione HTTP...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://consulting.local' -UseBasicParsing -TimeoutSec 10; if ($response.StatusCode -eq 200) { Write-Host '✓ Sito web raggiungibile: http://consulting.local' } else { Write-Host '⚠ Sito web risponde ma con errori (Status: ' + $response.StatusCode + ')' } } catch { Write-Host '✗ Sito web non raggiungibile: ' + $_.Exception.Message }"

echo.
echo ============================================
echo RIEPILOGO VERIFICA
echo ============================================
echo.
echo URL PRINCIPALE: http://consulting.local
echo DATABASE: SRV-dc\SQLEXPRESS\Consulting
echo AUTENTICAZIONE: Windows
echo.
echo CREDENZIALI PREDEFINITE:
echo 👤 admin / 123456 (Administrator)
echo 👤 senior / 123456 (UserSenior)  
echo 👤 user / 123456 (User)
echo.
echo PAGINE PRINCIPALI:
echo 🌐 http://consulting.local
echo 🔐 http://consulting.local/Account/Login
echo 👨‍💼 http://consulting.local/Admin
echo 🏦 http://consulting.local/DatiUtenza
echo 📁 http://consulting.local/DatiUtenzaExtra
echo.

pause
