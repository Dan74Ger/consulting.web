@echo off
REM Setup commit automatici per NUOVO PROGETTO

echo.
echo ========================================
echo   SETUP NUOVO PROGETTO - COMMIT AUTO
echo ========================================
echo.

echo Questo script configura commit automatici per un nuovo progetto.
echo.

REM Chiedi informazioni progetto
set /p project_path=Inserisci percorso completo progetto (es: C:\dev\nuovo_progetto): 
set /p github_user=Inserisci username GitHub: 
set /p github_repo=Inserisci nome repository GitHub: 
set /p frequency=Inserisci frequenza (30min, hourly, daily): 

echo.
echo ========================================
echo   CONFIGURAZIONE RILEVATA
echo ========================================
echo.
echo 📁 Percorso: %project_path%
echo 👤 GitHub User: %github_user%
echo 📦 Repository: %github_repo%
echo ⏰ Frequenza: %frequency%
echo 🌐 URL Completo: https://github.com/%github_user%/%github_repo%.git
echo.

set /p confirm=Confermi la configurazione? (s/n): 
if /i not "%confirm%"=="s" (
    echo Configurazione annullata.
    pause
    exit
)

echo.
echo ========================================
echo   CREAZIONE FILES PROGETTO
echo ========================================
echo.

REM Vai nella cartella del progetto
if not exist "%project_path%" (
    echo ❌ ERRORE: La cartella %project_path% non esiste!
    echo 💡 Crea prima la cartella del progetto.
    pause
    exit
)

cd /d "%project_path%"

REM Crea auto_commit.ps1 personalizzato
echo Creazione script auto_commit.ps1...
(
echo # Script per commit automatici - %github_repo%
echo # Configurato per: %project_path%
echo.
echo param^(
echo     [string]$CommitMessage = "Auto-commit: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
echo ^)
echo.
echo $Green = "Green"
echo $Red = "Red" 
echo $Yellow = "Yellow"
echo.
echo Write-Host "AVVIO COMMIT AUTOMATICO - %github_repo%..." -ForegroundColor $Yellow
echo Write-Host "Data: $(Get-Date)" -ForegroundColor $Yellow
echo Write-Host "Directory: $(Get-Location)" -ForegroundColor $Yellow
echo.
echo try {
echo     if ^(-not ^(Test-Path ".git"^)^) {
echo         Write-Host "ERRORE: Non siamo in un repository Git!" -ForegroundColor $Red
echo         exit 1
echo     }
echo.
echo     $status = git status --porcelain
echo     if ^(-not $status^) {
echo         Write-Host "Nessuna modifica da committare." -ForegroundColor $Yellow
echo         exit 0
echo     }
echo.
echo     Write-Host "Modifiche rilevate:" -ForegroundColor $Green
echo     Write-Host $status
echo.
echo     Write-Host "Aggiunta file..." -ForegroundColor $Yellow
echo     git add .
echo     
echo     if ^($LASTEXITCODE -ne 0^) {
echo         Write-Host "ERRORE durante git add" -ForegroundColor $Red
echo         exit 1
echo     }
echo.
echo     Write-Host "Esecuzione commit..." -ForegroundColor $Yellow
echo     git commit -m $CommitMessage
echo     
echo     if ^($LASTEXITCODE -ne 0^) {
echo         Write-Host "ERRORE durante git commit" -ForegroundColor $Red
echo         exit 1
echo     }
echo.
echo     Write-Host "Push su GitHub..." -ForegroundColor $Yellow
echo     git push origin master
echo     
echo     if ^($LASTEXITCODE -eq 0^) {
echo         Write-Host "COMMIT AUTOMATICO COMPLETATO CON SUCCESSO!" -ForegroundColor $Green
echo         Write-Host "Codice sincronizzato su GitHub" -ForegroundColor $Green
echo         
echo         $logMessage = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') - Auto-commit completato: $CommitMessage"
echo         Add-Content -Path "auto_commit.log" -Value $logMessage
echo     } else {
echo         Write-Host "ERRORE durante git push" -ForegroundColor $Red
echo         exit 1
echo     }
echo.
echo } catch {
echo     Write-Host "ERRORE CRITICO: $($_.Exception.Message)" -ForegroundColor $Red
echo     $errorMessage = "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') - ERRORE: $($_.Exception.Message)"
echo     Add-Content -Path "auto_commit_errors.log" -Value $errorMessage
echo     exit 1
echo }
echo.
echo Write-Host "Script terminato!" -ForegroundColor $Green
) > auto_commit.ps1

REM Crea batch per commit manuale
echo Creazione auto_commit.bat...
(
echo @echo off
echo REM Commit manuale per %github_repo%
echo echo.
echo echo ========================================
echo echo    COMMIT MANUALE - %github_repo%
echo echo ========================================
echo echo.
echo cd /d "%project_path%"
echo powershell -ExecutionPolicy Bypass -File "auto_commit.ps1"
echo echo.
echo echo Operazione completata!
echo pause
) > auto_commit.bat

REM Configura Git se necessario
echo Configurazione Git repository...
if not exist ".git" (
    echo Inizializzazione repository Git...
    git init
    git remote add origin https://github.com/%github_user%/%github_repo%.git
) else (
    echo Repository Git già esistente, aggiorno remote...
    git remote set-url origin https://github.com/%github_user%/%github_repo%.git
)

REM Determina parametri task scheduler
set task_name=GitHubAutoCommit_%github_repo%
if "%frequency%"=="30min" (
    set sched_params=/sc minute /mo 30
) else if "%frequency%"=="hourly" (
    set sched_params=/sc hourly
) else (
    set sched_params=/sc daily /st 18:00
)

echo.
echo ========================================
echo   CONFIGURAZIONE TASK SCHEDULER
echo ========================================
echo.

REM Rimuovi task esistenti
schtasks /delete /tn "%task_name%" /f >nul 2>&1

REM Crea nuovo task
echo Creazione task: %task_name%
schtasks /create ^
    /tn "%task_name%" ^
    /tr "powershell.exe -ExecutionPolicy Bypass -File \"%project_path%\auto_commit.ps1\"" ^
    %sched_params% ^
    /ru "%USERNAME%" ^
    /f

if %errorlevel%==0 (
    echo.
    echo ✅ PROGETTO CONFIGURATO CON SUCCESSO!
    echo.
    echo 📋 Task: %task_name%
    echo 📁 Percorso: %project_path%
    echo 🌐 Repository: https://github.com/%github_user%/%github_repo%
    echo ⏰ Frequenza: %frequency%
    echo.
    echo 🧪 ESECUZIONE TEST...
    schtasks /run /tn "%task_name%"
    echo.
    echo ✅ SETUP COMPLETATO!
    echo.
    echo 📋 Per gestire:
    echo    - Commit manuale: auto_commit.bat
    echo    - Task Scheduler: taskschd.msc
) else (
    echo ❌ ERRORE nella creazione del task!
    echo 💡 Esegui come Amministratore
)

echo.
echo ========================================
pause
