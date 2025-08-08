@echo off
REM =====================================================
REM SCRIPT DISINSTALLAZIONE - CONSULTING GROUP SRL
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
    echo Fare click destro sul file e selezionare "Esegui come amministratore"
    pause
    exit /b 1
)

set /p confirm="Sei sicuro di voler rimuovere l'applicazione? (S/N): "
if /i not "%confirm%"=="S" (
    echo Operazione annullata.
    pause
    exit /b 0
)

echo [1/4] Rimozione sito IIS...

REM Rimozione sito IIS
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo Rimuovendo sito ConsultingGroup...
    %windir%\system32\inetsrv\appcmd.exe delete site "ConsultingGroup"
    echo ✓ Sito IIS rimosso
) else (
    echo ✓ Sito IIS non presente
)

echo [2/4] Rimozione pool applicazioni...

REM Rimozione pool applicazioni
%windir%\system32\inetsrv\appcmd.exe list apppool "ConsultingGroup" >nul 2>&1
if %errorLevel% equ 0 (
    echo Rimuovendo pool ConsultingGroup...
    %windir%\system32\inetsrv\appcmd.exe delete apppool "ConsultingGroup"
    echo ✓ Pool applicazioni rimosso
) else (
    echo ✓ Pool applicazioni non presente
)

echo [3/4] Rimozione files...

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

echo [4/4] Verifica database...

echo.
echo ATTENZIONE: Il database NON è stato rimosso automaticamente.
echo Se necessario, rimuovere manualmente:
echo - Database: ConsultingGroupDB
echo - Login: consulting_user
echo.

echo ============================================
echo DISINSTALLAZIONE COMPLETATA!
echo ============================================

pause
