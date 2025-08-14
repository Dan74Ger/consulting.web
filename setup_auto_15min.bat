@echo off
REM Setup automatico per commit ogni 15 minuti (ultra-frequente)

echo.
echo ========================================
echo   SETUP COMMIT AUTOMATICI OGNI 15 MIN
echo ========================================
echo.

echo Configurazione: Ogni 15 minuti (ultra-backup)
echo ⚠️  ULTRA FREQUENTE: 4 controlli ogni ora!
echo    Solo per sviluppo molto intenso.
echo.

REM Rimuovi task esistenti se presenti
echo Rimozione task esistenti (se presenti)...
schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup*" /f >nul 2>&1

REM Crea il nuovo task ogni 15 minuti
echo Creazione task per commit ogni 15 minuti...
schtasks /create ^
    /tn "GitHubAutoCommit_ConsultingGroup_15min" ^
    /tr "powershell.exe -ExecutionPolicy Bypass -File \"C:\dev\prova\auto_commit.ps1\"" ^
    /sc minute ^
    /mo 15 ^
    /ru "%USERNAME%" ^
    /f

if %errorlevel%==0 (
    echo.
    echo ✅ TASK ULTRA-FREQUENTE CREATO!
    echo.
    echo 📋 Nome: GitHubAutoCommit_ConsultingGroup_15min
    echo 🕐 Frequenza: Ogni 15 minuti (4 volte/ora)
    echo 📁 Script: C:\dev\prova\auto_commit.ps1
    echo.
    echo ⚡⚡ BACKUP ULTRA-INTENSO ATTIVATO!
    echo     Massima protezione possibile.
    echo.
    echo 🧪 ESECUZIONE TEST...
    schtasks /run /tn "GitHubAutoCommit_ConsultingGroup_15min"
) else (
    echo.
    echo ❌ ERRORE durante la creazione del task!
    echo 💡 Esegui come Amministratore
)

echo.
echo ========================================
echo Premi un tasto per continuare...
pause >nul
