@echo off
REM Gestione completa task commit automatici

:menu
cls
echo.
echo ========================================
echo     GESTIONE COMMIT AUTOMATICI
echo ========================================
echo.
echo Task attualmente configurati:
echo.
schtasks /query /tn "GitHubAutoCommit*" 2>nul || echo Nessun task configurato
echo.
echo ========================================
echo.
echo 1. Setup OGNI 15 MINUTI (ultra-frequente)
echo 2. Setup OGNI 30 MINUTI (molto frequente)  ⭐ CONSIGLIATO PER SVILUPPO ATTIVO
echo 3. Setup OGNI ORA (frequente)
echo 4. Setup GIORNALIERO alle 18:00 (normale)
echo.
echo 5. COMMIT MANUALE IMMEDIATO
echo.
echo 6. VISUALIZZA stato task attuali
echo 7. RIMUOVI tutti i task automatici
echo 8. TEST esecuzione task attivo
echo.
echo 0. Esci
echo.
echo ========================================

set /p choice=Scegli opzione (0-8): 

if "%choice%"=="1" goto setup15
if "%choice%"=="2" goto setup30
if "%choice%"=="3" goto setuphour
if "%choice%"=="4" goto setupdaily
if "%choice%"=="5" goto manual
if "%choice%"=="6" goto status
if "%choice%"=="7" goto remove
if "%choice%"=="8" goto test
if "%choice%"=="0" goto exit
echo Scelta non valida!
pause
goto menu

:setup15
echo.
echo Configurazione OGNI 15 MINUTI...
call setup_auto_15min.bat
pause
goto menu

:setup30
echo.
echo Configurazione OGNI 30 MINUTI...
call setup_auto_30min.bat
pause
goto menu

:setuphour
echo.
echo Configurazione OGNI ORA...
call setup_auto_hourly.bat
pause
goto menu

:setupdaily
echo.
echo Configurazione GIORNALIERA...
call setup_auto_daily.bat
pause
goto menu

:manual
echo.
echo ========================================
echo      COMMIT MANUALE IMMEDIATO
echo ========================================
echo.
call auto_commit.bat
pause
goto menu

:status
echo.
echo ========================================
echo        STATO TASK ATTUALI
echo ========================================
echo.
schtasks /query /tn "GitHubAutoCommit*" /fo table /v 2>nul || echo Nessun task configurato
echo.
echo ========================================
pause
goto menu

:remove
echo.
echo ========================================
echo      RIMOZIONE TUTTI I TASK
echo ========================================
echo.
echo ⚠️  Stai per rimuovere TUTTI i task automatici!
set /p confirm=Sei sicuro? (s/n): 
if /i "%confirm%"=="s" (
    echo Rimozione in corso...
    schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup" /f >nul 2>&1
    schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup_Hourly" /f >nul 2>&1
    schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup_30min" /f >nul 2>&1
    schtasks /delete /tn "GitHubAutoCommit_ConsultingGroup_15min" /f >nul 2>&1
    echo ✅ Tutti i task sono stati rimossi!
) else (
    echo Operazione annullata.
)
pause
goto menu

:test
echo.
echo ========================================
echo         TEST ESECUZIONE
echo ========================================
echo.
echo Task disponibili per test:
schtasks /query /tn "GitHubAutoCommit*" 2>nul || echo Nessun task configurato
echo.
set /p tasktest=Inserisci nome completo task da testare: 
if not "%tasktest%"=="" (
    echo Esecuzione test di: %tasktest%
    schtasks /run /tn "%tasktest%"
    echo Test completato!
)
pause
goto menu

:exit
echo.
echo Arrivederci!
pause
exit
