@echo off
REM =====================================================
REM CONFIGURAZIONE FILE HOSTS PER consulting.local
REM =====================================================

echo.
echo ============================================
echo CONFIGURAZIONE FILE HOSTS
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

echo [1/4] Backup file hosts...

REM Backup del file hosts con timestamp
set timestamp=%date:~-4,4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%%time:~6,2%
set timestamp=%timestamp: =0%
copy "%windir%\System32\drivers\etc\hosts" "%windir%\System32\drivers\etc\hosts.backup.%timestamp%" >nul 2>&1
echo ✓ Backup creato: hosts.backup.%timestamp%

echo [2/4] Rimozione entry esistenti...

REM Creare file temporaneo senza le righe consulting.local
findstr /v "consulting.local" "%windir%\System32\drivers\etc\hosts" > "%temp%\hosts_new" 2>nul
if %errorLevel% equ 0 (
    echo ✓ Entry precedenti rimosse
) else (
    echo ⚠ Nessuna entry precedente trovata
    copy "%windir%\System32\drivers\etc\hosts" "%temp%\hosts_new" >nul 2>&1
)

echo [3/4] Aggiunta nuova entry...

REM Aggiungere la nuova riga
echo 127.0.0.1 consulting.local >> "%temp%\hosts_new"
echo ✓ Aggiunta: 127.0.0.1 consulting.local

echo [4/4] Applicazione modifiche...

REM Sostituire il file hosts originale
attrib -r "%windir%\System32\drivers\etc\hosts" >nul 2>&1
copy "%temp%\hosts_new" "%windir%\System32\drivers\etc\hosts" >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ File hosts aggiornato con successo
    del "%temp%\hosts_new" >nul 2>&1
    
    REM Flush DNS cache
    ipconfig /flushdns >nul 2>&1
    echo ✓ DNS cache pulito
) else (
    echo ✗ Errore nell'aggiornamento del file hosts
    echo.
    echo SOLUZIONE MANUALE:
    echo 1. Aprire Notepad come Amministratore
    echo 2. Aprire file: %windir%\System32\drivers\etc\hosts
    echo 3. Aggiungere alla fine: 127.0.0.1 consulting.local
    echo 4. Salvare il file
    pause
    exit /b 1
)

echo.
echo ============================================
echo CONFIGURAZIONE COMPLETATA!
echo ============================================
echo.
echo ✓ consulting.local configurato
echo ✓ Risolve su: 127.0.0.1
echo.
echo VERIFICA:
ping consulting.local -n 1 >nul 2>&1
if %errorLevel% equ 0 (
    echo ✓ consulting.local risolve correttamente
) else (
    echo ⚠ consulting.local potrebbe non risolvere
)

echo.
echo CONTENUTO FILE HOSTS:
echo ----------------------------------------
findstr "consulting.local" "%windir%\System32\drivers\etc\hosts"
echo ----------------------------------------
echo.

pause
