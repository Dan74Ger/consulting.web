@echo off
REM Setup automatico per commit giornalieri alle 18:00

echo.
echo ========================================
echo   SETUP COMMIT AUTOMATICI GIORNALIERI
echo ========================================
echo.

echo Configurazione: Giornaliero alle 18:00
echo.

REM Rimuovi task esistente se presente
echo Rimozione task esistente (se presente)...
schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup" /f >nul 2>&1

REM Crea il nuovo task
echo Creazione task per commit automatici...
schtasks /create ^
    /tn "GitHubAutoCommit_ConsultingGroup" ^
    /tr "powershell.exe -ExecutionPolicy Bypass -File \"C:\dev\prova\auto_commit.ps1\"" ^
    /sc daily ^
    /st 18:00 ^
    /ru "%USERNAME%" ^
    /f

if %errorlevel%==0 (
    echo.
    echo ✅ TASK CREATO CON SUCCESSO!
    echo.
    echo 📋 Nome: GitHubAutoCommit_ConsultingGroup
    echo 🕐 Orario: Giornaliero alle 18:00
    echo 📁 Script: C:\dev\prova\auto_commit.ps1
    echo.
    echo 🧪 ESECUZIONE TEST...
    schtasks /run /tn "GitHubAutoCommit_ConsultingGroup"
    echo.
    echo ✅ Test completato! Controlla se ha funzionato.
    echo.
    echo 🔧 Per gestire il task:
    echo    Windows + R → taskschd.msc
    echo    Cerca: GitHubAutoCommit_ConsultingGroup
) else (
    echo.
    echo ❌ ERRORE durante la creazione del task!
    echo 💡 Prova ad eseguire questo script come Amministratore:
    echo    Tasto destro → "Esegui come amministratore"
)

echo.
echo ========================================
echo Premi un tasto per continuare...
pause >nul
