@echo off
REM Clona configurazione commit automatici in altro progetto

echo.
echo ========================================
echo   CLONA CONFIGURAZIONE COMMIT AUTO
echo ========================================
echo.

echo Questo script copia la configurazione attuale in un nuovo progetto.
echo.

set /p target_path=Inserisci percorso nuovo progetto: 
set /p new_repo=Inserisci URL repository GitHub (es: https://github.com/user/repo.git): 

echo.
echo Copia in corso...

REM Crea cartella se non esiste
if not exist "%target_path%" (
    mkdir "%target_path%"
)

REM Copia files di configurazione
copy "auto_commit.ps1" "%target_path%\"
copy "auto_commit.bat" "%target_path%\"
copy "gestione_task.bat" "%target_path%\"
copy "setup_auto_30min.bat" "%target_path%\"
copy "setup_auto_daily.bat" "%target_path%\"
copy "setup_auto_hourly.bat" "%target_path%\"

echo.
echo ✅ File copiati!
echo.
echo 🔧 PROSSIMI PASSI:
echo 1. Vai in: %target_path%
echo 2. Modifica repository in auto_commit.ps1
echo 3. Esegui setup_auto_30min.bat
echo.

pause
