@echo off
REM =====================================================
REM DISINSTALLAZIONE SERVER - CONSULTING GROUP SRL
REM =====================================================

echo.
echo ============================================
echo DISINSTALLAZIONE GESTIONE STUDIO - CONSULTING GROUP SRL
echo ============================================
echo.

REM Verifica privilegi amministratore
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: Eseguire come Amministratore!
    pause
    exit /b 1
)

set /p confirm="Sei sicuro di voler rimuovere l'applicazione? (S/N): "
if /i not "%confirm%"=="S" (
    echo Operazione annullata.
    pause
    exit /b 0
)

echo [1/5] Rimozione sito IIS...

REM Rimozione sito IIS
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo Rimuovendo sito ConsultingGroup...
    %windir%\system32\inetsrv\appcmd.exe delete site "ConsultingGroup"
    echo ✓ Sito IIS rimosso
) else (
    echo ✓ Sito IIS non presente
)

echo [2/5] Rimozione pool applicazioni...

REM Rimozione pool applicazioni
%windir%\system32\inetsrv\appcmd.exe list apppool "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo Rimuovendo pool ConsultingGroup...
    %windir%\system32\inetsrv\appcmd.exe delete apppool "ConsultingGroup"
    echo ✓ Pool applicazioni rimosso
) else (
    echo ✓ Pool applicazioni non presente
)

echo [3/5] Pulizia file hosts...

REM Rimozione da file hosts
findstr "consulting.local" %windir%\System32\drivers\etc\hosts >nul 2>&1
if %errorLevel% equ 0 (
    echo Rimuovendo consulting.local da file hosts...
    powershell -Command "(Get-Content '%windir%\System32\drivers\etc\hosts') | Where-Object { $_ -notmatch 'consulting.local' } | Set-Content '%windir%\System32\drivers\etc\hosts'"
    echo ✓ consulting.local rimosso da file hosts
) else (
    echo ✓ consulting.local non presente in file hosts
)

echo [4/5] Rimozione files...

set /p removefiles="Rimuovere anche i files dell'applicazione? (S/N): "
if /i "%removefiles%"=="S" (
    if exist "C:\inetpub\consulting.web" (
        echo Rimuovendo files da C:\inetpub\consulting.web...
        rmdir /s /q "C:\inetpub\consulting.web"
        echo ✓ Files applicazione rimossi
    )
    
    if exist "C:\inetpub\logs\consulting" (
        echo Rimuovendo logs da C:\inetpub\logs\consulting...
        rmdir /s /q "C:\inetpub\logs\consulting"
        echo ✓ Files log rimossi
    )
) else (
    echo ✓ Files mantenuti
)

echo [5/5] Verifica database...

set /p removedb="Rimuovere anche il database Consulting? (S/N): "
if /i "%removedb%"=="S" (
    echo Rimuovendo database Consulting...
    sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "DROP DATABASE IF EXISTS Consulting" >nul 2>&1
    if %errorLevel% equ 0 (
        echo ✓ Database Consulting rimosso
    ) else (
        echo ⚠ Errore nella rimozione database
    )
) else (
    echo ✓ Database mantenuto
)

REM Flush DNS
ipconfig /flushdns >nul 2>&1

echo.
echo ============================================
echo DISINSTALLAZIONE COMPLETATA!
echo ============================================
echo.
echo ✓ Sito IIS rimosso
echo ✓ Pool applicazioni rimosso  
echo ✓ File hosts pulito
if /i "%removefiles%"=="S" echo ✓ Files applicazione rimossi
if /i "%removedb%"=="S" echo ✓ Database rimosso
echo ✓ DNS cache pulito
echo.

pause
