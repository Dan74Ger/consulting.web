@echo off
REM Setup automatico per commit ogni ora

echo.
echo ========================================
echo   SETUP COMMIT AUTOMATICI OGNI ORA
echo ========================================
echo.

echo Configurazione: Ogni ora (sviluppo intenso)
echo.

REM Rimuovi task esistente se presente
echo Rimozione task esistente (se presente)...
schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup_Hourly" /f >nul 2>&1

REM Crea il nuovo task
echo Creazione task per commit ogni ora...
schtasks /create ^
    /tn "GitHubAutoCommit_ConsultingGroup_Hourly" ^
    /tr "powershell.exe -ExecutionPolicy Bypass -File \"C:\dev\prova\auto_commit.ps1\"" ^
    /sc hourly ^
    /ru "%USERNAME%" ^
    /f

if %errorlevel%==0 (
    echo.
    echo ✅ TASK CREATO CON SUCCESSO!
    echo.
    echo 📋 Nome: GitHubAutoCommit_ConsultingGroup_Hourly
    echo 🕐 Frequenza: Ogni ora
    echo 📁 Script: C:\dev\prova\auto_commit.ps1
    echo.
    echo ⚠️  ATTENZIONE: Questo farà commit ogni ora!
    echo    Perfetto per sviluppo intenso.
    echo.
    echo 🧪 ESECUZIONE TEST...
    schtasks /run /tn "GitHubAutoCommit_ConsultingGroup_Hourly"
    echo.
    echo ✅ Test completato! Controlla se ha funzionato.
) else (
    echo.
    echo ❌ ERRORE durante la creazione del task!
    echo 💡 Prova ad eseguire questo script come Amministratore
)

echo.
echo ========================================
echo Premi un tasto per continuare...
pause >nul
