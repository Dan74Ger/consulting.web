@echo off
REM Script per configurare Task Scheduler - Versione Sicura
REM Questo script funziona anche con ExecutionPolicy Restricted

echo.
echo ========================================
echo   CONFIGURAZIONE COMMIT AUTOMATICI
echo ========================================
echo.

echo Scegli la frequenza:
echo 1. Giornaliero alle 18:00 (CONSIGLIATO)
echo 2. Ogni ora (sviluppo intenso)
echo 3. Settimanale Lun-Mer-Ven alle 18:00
echo 4. Personalizzato
echo.

set /p choice=Inserisci numero scelta (1-4): 

if "%choice%"=="1" goto daily
if "%choice%"=="2" goto hourly
if "%choice%"=="3" goto weekly
if "%choice%"=="4" goto custom
echo Scelta non valida!
pause
exit

:daily
set taskname=GitHubAutoCommit_ConsultingGroup_Daily
set trigger=/sc daily /st 18:00
echo Configurazione: Giornaliera alle 18:00
goto create

:hourly
set taskname=GitHubAutoCommit_ConsultingGroup_Hourly
set trigger=/sc hourly
echo Configurazione: Ogni ora
goto create

:weekly
set taskname=GitHubAutoCommit_ConsultingGroup_Weekly
set trigger=/sc weekly /d MON,WED,FRI /st 18:00
echo Configurazione: Settimanale Lun-Mer-Ven alle 18:00
goto create

:custom
echo.
set /p hour=Inserisci ora (formato 24h, es. 17:30): 
set /p freq=Inserisci frequenza (daily/weekly): 
set taskname=GitHubAutoCommit_ConsultingGroup_Custom
if "%freq%"=="daily" (
    set trigger=/sc daily /st %hour%
) else (
    set trigger=/sc weekly /d MON,WED,FRI /st %hour%
)

:create
echo.
echo ========================================
echo   CREAZIONE TASK SCHEDULER
echo ========================================
echo.

REM Rimuovi task esistente se presente
echo Rimozione task esistente (se presente)...
schtasks /delete /tn "%taskname%" /f >nul 2>&1

REM Crea il nuovo task
echo Creazione nuovo task: %taskname%
schtasks /create /tn "%taskname%" /tr "powershell.exe -ExecutionPolicy Bypass -File \"C:\dev\prova\auto_commit.ps1\"" /sc daily /st 18:00 %trigger% /ru "%USERNAME%" /f

if %errorlevel%==0 (
    echo.
    echo ✅ TASK CREATO CON SUCCESSO!
    echo.
    echo 📋 Nome task: %taskname%
    echo 🕐 Schedulazione: %trigger%
    echo 📁 Script: C:\dev\prova\auto_commit.ps1
    echo.
    echo 🔧 GESTIONE TASK:
    echo - Aprire: Pannello di controllo ^> Strumenti amministrativi ^> Utilità di pianificazione
    echo - Cercare: %taskname%
    echo.
    echo 🧪 TEST IMMEDIATO:
    schtasks /run /tn "%taskname%"
    echo Task avviato manualmente per test!
) else (
    echo.
    echo ❌ ERRORE durante la creazione del task!
    echo 💡 Assicurati di eseguire questo script come Amministratore
)

echo.
echo ========================================
echo Premi un tasto per continuare...
pause >nul
