@echo off
REM =====================================================
REM INSTALLAZIONE COMPLETA SERVER - CONSULTING GROUP SRL
REM Database: SRV-dc\SQLEXPRESS - Consulting - Windows Auth
REM URL: http://consulting.local
REM =====================================================

echo.
echo ============================================
echo INSTALLAZIONE GESTIONE STUDIO - CONSULTING GROUP SRL
echo Database: SRV-dc\SQLEXPRESS
echo Database Name: Consulting
echo URL: http://consulting.local
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

echo [1/10] Verifica prerequisiti...

REM Verifica IIS
sc query W3SVC >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: IIS non installato!
    echo Installare IIS prima di continuare.
    pause
    exit /b 1
)

REM Verifica .NET 9.0 Runtime
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App 9." >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: .NET 9.0 ASP.NET Core Runtime non installato!
    echo Scaricare da: https://dotnet.microsoft.com/download/dotnet/9.0
    pause
    exit /b 1
)

echo [2/10] Test connessione database...

REM Test connessione al database
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "SELECT @@VERSION" >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: Impossibile connettersi a SRV-dc\SQLEXPRESS
    echo Verificare:
    echo - SQL Server Express in esecuzione
    echo - Nome server corretto
    echo - Permessi Windows Authentication
    pause
    exit /b 1
) else (
    echo ✓ Connessione a SRV-dc\SQLEXPRESS riuscita
)

echo [3/10] Creazione cartelle...

REM Creazione cartelle
if not exist "C:\inetpub\consulting.web" mkdir "C:\inetpub\consulting.web"
if not exist "C:\inetpub\logs\consulting" mkdir "C:\inetpub\logs\consulting"

echo [4/10] Copia files applicazione...

REM Copia files dalla cartella publish
if exist "publish" (
    echo Copiando files da cartella publish...
    xcopy "publish\*" "C:\inetpub\consulting.web\" /E /Y /I
) else (
    echo ERRORE: Cartella publish non trovata!
    echo Eseguire prima: dotnet publish -c Release -o publish
    pause
    exit /b 1
)

echo [5/10] Configurazione database...

REM Creazione database se non esiste
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'Consulting') CREATE DATABASE Consulting"
if %errorLevel% neq 0 (
    echo ERRORE: Impossibile creare database Consulting
    pause
    exit /b 1
) else (
    echo ✓ Database Consulting verificato/creato
)

echo [6/10] Configurazione applicazione...

REM Creazione file configurazione server
(
echo {
echo   "Logging": {
echo     "LogLevel": {
echo       "Default": "Warning",
echo       "Microsoft.AspNetCore": "Warning",
echo       "Microsoft.EntityFrameworkCore": "Warning"
echo     }
echo   },
echo   "ConnectionStrings": {
echo     "DefaultConnection": "Server=SRV-dc\\SQLEXPRESS;Database=Consulting;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;Connection Timeout=30;"
echo   },
echo   "AllowedHosts": "*",
echo   "Environment": "Production",
echo   "Application": {
echo     "Name": "GESTIONE STUDIO - CONSULTING GROUP SRL",
echo     "Version": "1.0.0",
echo     "Environment": "Production",
echo     "BaseUrl": "http://consulting.local"
echo   },
echo   "ASPNETCORE_ENVIRONMENT": "Production"
echo }
) > "C:\inetpub\consulting.web\appsettings.Production.json"

echo [7/10] Impostazione permessi...

REM Impostazione permessi
icacls "C:\inetpub\consulting.web" /grant "IIS_IUSRS:(OI)(CI)RX" /T
icacls "C:\inetpub\logs\consulting" /grant "IIS_IUSRS:(OI)(CI)F" /T
icacls "C:\inetpub\logs\consulting" /grant "NETWORK SERVICE:(OI)(CI)F" /T

echo [8/10] Configurazione IIS per consulting.local...

REM Rimozione sito esistente se presente
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo Rimuovendo sito esistente...
    %windir%\system32\inetsrv\appcmd.exe delete site "ConsultingGroup"
)

REM Creazione nuovo sito con binding consulting.local
%windir%\system32\inetsrv\appcmd.exe add site /name:"ConsultingGroup" /physicalPath:"C:\inetpub\consulting.web" /bindings:http/*:80:consulting.local

REM Configurazione pool applicazioni
%windir%\system32\inetsrv\appcmd.exe set apppool "ConsultingGroup" /managedRuntimeVersion:
%windir%\system32\inetsrv\appcmd.exe set apppool "ConsultingGroup" /processModel.identityType:ApplicationPoolIdentity
%windir%\system32\inetsrv\appcmd.exe set apppool "ConsultingGroup" /recycling.periodicRestart.time:00:00:00

REM Impostazione variabili ambiente
%windir%\system32\inetsrv\appcmd.exe set config "ConsultingGroup" /section:appSettings /+"[key='ASPNETCORE_ENVIRONMENT',value='Production']"

echo [9/10] Configurazione file hosts...

REM Backup del file hosts
copy "%windir%\System32\drivers\etc\hosts" "%windir%\System32\drivers\etc\hosts.backup" >nul 2>&1

REM Metodo sicuro per modificare file hosts
echo Configurando consulting.local nel file hosts...
findstr /v "consulting.local" "%windir%\System32\drivers\etc\hosts" > "%temp%\hosts_new" 2>nul
echo 127.0.0.1 consulting.local >> "%temp%\hosts_new"

REM Rimuovere attributo read-only e sostituire file
attrib -r "%windir%\System32\drivers\etc\hosts" >nul 2>&1
copy "%temp%\hosts_new" "%windir%\System32\drivers\etc\hosts" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ File hosts configurato con successo
    del "%temp%\hosts_new" >nul 2>&1
) else (
    echo ⚠ Errore configurazione file hosts
    echo   Eseguire manualmente: CONFIGURE_HOSTS.bat
    del "%temp%\hosts_new" >nul 2>&1
)

echo [10/10] Configurazione database e avvio servizi...

REM Restart servizi per applicare configurazioni
echo Restart servizi IIS...
%windir%\system32\inetsrv\appcmd.exe stop site "ConsultingGroup" >nul 2>&1
%windir%\system32\inetsrv\appcmd.exe start site "ConsultingGroup"

REM Il database verrà inizializzato automaticamente al primo accesso web
echo ✓ Database sarà inizializzato automaticamente al primo accesso

ipconfig /flushdns >nul 2>&1

REM Tornare alla cartella originale
cd /d "%~dp0"

echo.
echo ============================================
echo INSTALLAZIONE COMPLETATA!
echo ============================================
echo.
echo ✓ Database: SRV-dc\SQLEXPRESS\Consulting
echo ✓ Autenticazione: Windows
echo ✓ URL: http://consulting.local
echo ✓ Files copiati in: C:\inetpub\consulting.web
echo ✓ IIS configurato
echo ✓ File hosts aggiornato
echo.
echo VERIFICA INSTALLAZIONE:
echo.

REM Verifica finale
if exist "C:\inetpub\consulting.web\ConsultingGroup.dll" (
    echo ✓ Assembly principale presente
) else (
    echo ✗ Assembly principale mancante
)

if exist "C:\inetpub\consulting.web\Views\Home\Index.cshtml" (
    echo ✓ Views presenti
) else (
    echo ✗ Views mancanti
)

REM Test connessione database finale
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT 'Database OK' as Status" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ Database Consulting accessibile
) else (
    echo ⚠ Verificare accesso database Consulting
)

echo.
echo CONFIGURAZIONE IIS:
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup"

echo.
echo ============================================
echo ACCESSO AL SISTEMA
echo ============================================
echo.
echo 🌐 URL: http://consulting.local
echo 🔐 Login: http://consulting.local/Account/Login
echo.
echo 👤 UTENTI PREDEFINITI:
echo    - Administrator: admin / 123456
echo    - Senior User: senior / 123456
echo    - Basic User: user / 123456
echo.
echo ⚠️  IMPORTANTE: Cambiare le password predefinite!
echo.
echo PROSSIMI PASSI:
echo 1. Eseguire migration database: DATABASE_MIGRATION.bat
echo 2. Aprire browser: http://consulting.local
echo 3. Testare login con admin/123456
echo 4. Verificare tutte le funzionalità
echo 5. Cambiare password utenti
echo.
echo NOTA: Il database sarà inizializzato al primo accesso web.
echo Se serve migration manuale, usare: DATABASE_MIGRATION.bat
echo.
echo LOG APPLICAZIONE: C:\inetpub\logs\consulting\
echo.

pause
