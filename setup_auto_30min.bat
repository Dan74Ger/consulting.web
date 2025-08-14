@echo off
REM Setup automatico per commit ogni 30 minuti

echo.
echo ========================================
echo   SETUP COMMIT AUTOMATICI OGNI 30 MIN
echo ========================================
echo.

echo Configurazione: Ogni 30 minuti (backup continuo)
echo ⚠️  ATTENZIONE: Questo farà molti commit!
echo    Perfetto per sviluppo attivo continuo.
echo.

REM Rimuovi task esistenti se presenti
echo Rimozione task esistenti (se presenti)...
schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup" /f >nul 2>&1
schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup_Hourly" /f >nul 2>&1
schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup_30min" /f >nul 2>&1

REM Crea il nuovo task ogni 30 minuti
echo Creazione task per commit ogni 30 minuti...
schtasks /create ^
    /tn "GitHubAutoCommit_ConsultingGroup_30min" ^
    /tr "powershell.exe -ExecutionPolicy Bypass -File \"C:\dev\prova\auto_commit.ps1\"" ^
    /sc minute ^
    /mo 30 ^
    /ru "%USERNAME%" ^
    /f

if %errorlevel%==0 (
    echo.
    echo ✅ TASK CREATO CON SUCCESSO!
    echo.
    echo 📋 Nome: GitHubAutoCommit_ConsultingGroup_30min
    echo 🕐 Frequenza: Ogni 30 minuti
    echo 📁 Script: C:\dev\prova\auto_commit.ps1
    echo.
    echo ⚡ BACKUP ULTRA-FREQUENTE ATTIVATO!
    echo   Il tuo codice sarà salvato ogni 30 minuti.
    echo   Solo se ci sono modifiche effettive.
    echo.
    echo 🧪 ESECUZIONE TEST...
    schtasks /run /tn "GitHubAutoCommit_ConsultingGroup_30min"
    echo.
    echo ✅ Test completato! 
    echo.
    echo 📊 PROSSIME ESECUZIONI:
    schtasks /query /tn "GitHubAutoCommit_ConsultingGroup_30min"
    echo.
    echo 🔧 Per gestire il task:
    echo    Windows + R → taskschd.msc
    echo    Cerca: GitHubAutoCommit_ConsultingGroup_30min
    echo.
    echo 🛑 Per disattivare:
    echo    schtasks /end /tn "GitHubAutoCommit_ConsultingGroup_30min"
    echo    schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup_30min" /f
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
