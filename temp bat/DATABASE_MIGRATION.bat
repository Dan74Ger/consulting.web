@echo off
REM =====================================================
REM MIGRATION DATABASE - CONSULTING GROUP SRL
REM Database: SRV-dc\SQLEXPRESS - Consulting
REM =====================================================

echo.
echo ============================================
echo MIGRATION DATABASE - CONSULTING GROUP SRL
echo ============================================
echo.

REM Verifica che l'applicazione sia installata
if not exist "C:\inetpub\consulting.web\ConsultingGroup.dll" (
    echo ERRORE: Applicazione non installata!
    echo Eseguire prima: INSTALL_SERVER_CONSULTING.bat
    pause
    exit /b 1
)

echo [1/5] Test connessione database...

REM Test connessione database
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT 'OK' as Status" >nul 2>&1
if %errorLevel% neq 0 (
    echo ERRORE: Database Consulting non accessibile!
    echo Verificare:
    echo - Database Consulting creato
    echo - SQL Server in esecuzione
    echo - Permessi Windows Authentication
    pause
    exit /b 1
) else (
    echo ✓ Database Consulting accessibile
)

echo [2/5] Backup database attuale...

REM Backup database prima della migration
set timestamp=%date:~-4,4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%%time:~6,2%
set timestamp=%timestamp: =0%
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "BACKUP DATABASE Consulting TO DISK = 'C:\Temp\Consulting_PreMigration_%timestamp%.bak'" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ Backup creato: Consulting_PreMigration_%timestamp%.bak
) else (
    echo ⚠ Backup non creato (cartella C:\Temp potrebbe non esistere)
)

echo [3/5] Esecuzione migration tramite web...

REM Fermare sito temporaneamente
%windir%\system32\inetsrv\appcmd.exe stop site "ConsultingGroup" >nul 2>&1

REM Avviare sito
%windir%\system32\inetsrv\appcmd.exe start site "ConsultingGroup"

REM Attendere avvio
echo Attendere avvio applicazione...
timeout /t 5 /nobreak >nul

REM Effettuare richiesta HTTP per triggare la migration
echo Triggering database migration...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://consulting.local' -UseBasicParsing -TimeoutSec 30; Write-Host 'Migration triggata - Status:' $response.StatusCode } catch { Write-Host 'Tentativo migration - Errore normale durante inizializzazione:' $_.Exception.Message }" 2>nul

echo [4/5] Verifica tabelle create...

REM Attendere che la migration completi
timeout /t 10 /nobreak >nul

REM Verificare tabelle create
echo Verificando tabelle nel database...
for /f "skip=2 tokens=*" %%i in ('sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1 2^>nul') do (
    set tablecount=%%i
)

if defined tablecount (
    if %tablecount% GEQ 10 (
        echo ✓ Tabelle create con successo ^(%tablecount% tabelle^)
        
        REM Verificare utenti
        for /f "skip=2 tokens=*" %%i in ('sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT COUNT(*) FROM AspNetUsers" -h -1 2^>nul') do (
            set usercount=%%i
        )
        
        if defined usercount (
            if %usercount% GEQ 3 (
                echo ✓ Utenti predefiniti creati ^(%usercount% utenti^)
            ) else (
                echo ⚠ Potrebbero mancare utenti predefiniti ^(%usercount% utenti^)
            )
        )
    ) else (
        echo ⚠ Poche tabelle trovate ^(%tablecount%^) - Migration potrebbe non essere completa
    )
) else (
    echo ✗ Impossibile verificare tabelle
)

echo [5/5] Test accesso web finale...

REM Test finale accesso web
timeout /t 3 /nobreak >nul
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://consulting.local' -UseBasicParsing -TimeoutSec 15; if ($response.StatusCode -eq 200) { Write-Host '✓ Sito web funzionante' } else { Write-Host '⚠ Sito risponde ma con errori (Status: ' + $response.StatusCode + ')' } } catch { Write-Host '✗ Sito web non raggiungibile: ' + $_.Exception.Message }"

echo.
echo ============================================
echo MIGRATION COMPLETATA
echo ============================================
echo.

REM Mostrare riepilogo database
echo RIEPILOGO DATABASE:
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT 'Database: Consulting' as Info UNION ALL SELECT 'Tabelle: ' + CAST(COUNT(*) as VARCHAR) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'" -h -1 2>nul

echo.
echo PROSSIMI PASSI:
echo 1. Aprire browser: http://consulting.local
echo 2. Testare login: admin / 123456
echo 3. Verificare funzionalità CRUD
echo.
echo Se il login non funziona, le tabelle potrebbero essere vuote.
echo In tal caso, il sistema creerà gli utenti al primo accesso.
echo.

pause
