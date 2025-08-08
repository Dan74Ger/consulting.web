@echo off
REM =====================================================
REM SCRIPT INSTALLAZIONE AUTOMATICA - CONSULTING GROUP SRL
REM =====================================================

echo.
echo ============================================
echo INSTALLAZIONE GESTIONE STUDIO - CONSULTING GROUP SRL
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

echo [1/8] Verifica prerequisiti...

REM Verifica IIS
sc query W3SVC >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: IIS non installato!
    echo Installare IIS prima di continuare.
    pause
    exit /b 1
)

echo [2/8] Creazione cartelle...

REM Creazione cartelle
if not exist "C:\inetpub\consulting.web" mkdir "C:\inetpub\consulting.web"
if not exist "C:\inetpub\logs\consulting" mkdir "C:\inetpub\logs\consulting"

echo [3/8] Copia files applicazione...

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

echo [4/8] Copia configurazione server...

REM Copia configurazione server
if exist "appsettings.Server.json" (
    copy "appsettings.Server.json" "C:\inetpub\consulting.web\appsettings.Production.json" /Y
)

echo [5/8] Impostazione permessi...

REM Impostazione permessi
icacls "C:\inetpub\consulting.web" /grant "IIS_IUSRS:(OI)(CI)RX" /T
icacls "C:\inetpub\logs\consulting" /grant "IIS_IUSRS:(OI)(CI)F" /T
icacls "C:\inetpub\logs\consulting" /grant "NETWORK SERVICE:(OI)(CI)F" /T

echo [6/8] Creazione sito IIS...

REM Verifica se sito esiste già
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo Rimuovendo sito esistente...
    %windir%\system32\inetsrv\appcmd.exe delete site "ConsultingGroup"
)

REM Creazione nuovo sito
%windir%\system32\inetsrv\appcmd.exe add site /name:"ConsultingGroup" /physicalPath:"C:\inetpub\consulting.web" /bindings:http/*:80:

echo [7/8] Configurazione pool applicazioni...

REM Configurazione pool
%windir%\system32\inetsrv\appcmd.exe set apppool "ConsultingGroup" /managedRuntimeVersion:
%windir%\system32\inetsrv\appcmd.exe set apppool "ConsultingGroup" /processModel.identityType:ApplicationPoolIdentity
%windir%\system32\inetsrv\appcmd.exe set apppool "ConsultingGroup" /recycling.periodicRestart.time:00:00:00

REM Impostazione variabili ambiente
%windir%\system32\inetsrv\appcmd.exe set config "ConsultingGroup" /section:appSettings /+"[key='ASPNETCORE_ENVIRONMENT',value='Production']"

echo [8/8] Test installazione...

REM Test base
if exist "C:\inetpub\consulting.web\ConsultingGroup.dll" (
    echo ✓ File applicazione presente
) else (
    echo ✗ File applicazione mancante
)

if exist "C:\inetpub\consulting.web\Views\Home\Index.cshtml" (
    echo ✓ Views presenti
) else (
    echo ✗ Views mancanti
)

echo.
echo ============================================
echo INSTALLAZIONE COMPLETATA!
echo ============================================
echo.
echo PROSSIMI PASSI:
echo 1. Configurare database SQL Server
echo 2. Eseguire script: SCRIPTS_INSTALLAZIONE.sql
echo 3. Aggiornare stringa connessione in appsettings.Production.json
echo 4. Aprire browser: http://localhost
echo 5. Login: admin/123456
echo.
echo DOCUMENTI UTILI:
echo - INSTALLAZIONE_SERVER.md: Guida completa
echo - SCRIPTS_INSTALLAZIONE.sql: Script database
echo.
echo ============================================

pause
