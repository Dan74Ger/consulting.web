@echo off
setlocal enabledelayedexpansion

echo ============================================
echo BACKUP AUTOMATICO DATABASE E CODICE SITO
echo ============================================
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo Codice sorgente: C:\dev\prova
echo.

REM ===== RILEVA VERSIONE DATABASE =====
set "max_db_ver=0"
for /d %%i in ("C:\dev\vprova_backup_ver*_database") do (
    set "folder=%%~ni"
    set "folder=!folder:vprova_backup_ver=!"
    set "folder=!folder:_database=!"
    if !folder! gtr !max_db_ver! set "max_db_ver=!folder!"
)

set /a "new_db_ver=max_db_ver+1"

REM ===== RILEVA VERSIONE CODICE =====
set "max_code_ver=0"
for /d %%i in ("C:\dev\vprova_backup_ver*_codice") do (
    set "folder=%%~ni"
    set "folder=!folder:vprova_backup_ver=!"
    set "folder=!folder:_codice=!"
    if !folder! gtr !max_code_ver! set "max_code_ver=!folder!"
)

set /a "new_code_ver=max_code_ver+1"

REM ===== MOSTRA INFORMAZIONI =====
echo 📋 INFORMAZIONI VERSIONI:
echo    Ultima versione database: %max_db_ver%
echo    Prossima versione database: %new_db_ver%
echo    Ultima versione codice: %max_code_ver%
echo    Prossima versione codice: %new_code_ver%
echo.

set "db_dir=C:\dev\vprova_backup_ver%new_db_ver%_database"
set "code_dir=C:\dev\vprova_backup_ver%new_code_ver%_codice"
set "db_file=%db_dir%\Consulting_backup_ver%new_db_ver%.bak"

echo 📁 DESTINAZIONI BACKUP:
echo    Database: %db_file%
echo    Codice: %code_dir%
echo.

REM ===== VERIFICA PERMESSI DIRECTORY =====
echo 🔍 Verifica permessi creazione directory...
set "test_dir=C:\dev\test_temp_permissions"
md "%test_dir%" 2>nul
if exist "%test_dir%" (
    rd "%test_dir%" 2>nul
    echo ✓ Permessi directory OK
) else (
    echo ❌ ERRORE: Impossibile creare directory in C:\dev\
    echo    Verifica i permessi di scrittura nella directory C:\dev\
    pause
    exit /b 1
)
echo.

REM ===== CHIEDI CONFERMA DATABASE =====
echo ❓ Vuoi fare il backup del DATABASE? (S/N)
set /p "choice_db=Risposta: "
if /i not "%choice_db%"=="S" if /i not "%choice_db%"=="Si" (
    echo ⏭ Backup database saltato.
    set "do_db_backup=false"
) else (
    echo ✓ Backup database confermato.
    set "do_db_backup=true"
)
echo.

REM ===== CHIEDI CONFERMA CODICE =====
echo ❓ Vuoi fare il backup del CODICE SORGENTE? (S/N)
set /p "choice_code=Risposta: "
if /i not "%choice_code%"=="S" if /i not "%choice_code%"=="Si" (
    echo ⏭ Backup codice saltato.
    set "do_code_backup=false"
) else (
    echo ✓ Backup codice confermato.
    set "do_code_backup=true"
)
echo.

REM ===== VERIFICA SE FARE QUALCOSA =====
if "%do_db_backup%"=="false" if "%do_code_backup%"=="false" (
    echo ⚠ Nessun backup selezionato. Uscita...
    pause
    exit /b 0
)

REM ===== RIEPILOGO FINALE =====
echo ==========================================
echo 📋 RIEPILOGO OPERAZIONI
echo ==========================================
if "%do_db_backup%"=="true" (
    echo ✓ Database ver%new_db_ver%: %db_file%
)
if "%do_code_backup%"=="true" (
    echo ✓ Codice ver%new_code_ver%: %code_dir%
)
echo.
echo ❓ Confermi di procedere con i backup? (S/N)
set /p "final_confirm=Risposta finale: "
if /i not "%final_confirm%"=="S" if /i not "%final_confirm%"=="Si" (
    echo ❌ Operazione annullata dall'utente.
    pause
    exit /b 0
)

echo.
echo 🚀 Avvio backup...
echo.

REM ===== BACKUP DATABASE =====
if "%do_db_backup%"=="true" (
    echo ==========================================
    echo 💾 BACKUP DATABASE - VERSIONE %new_db_ver%
    echo ==========================================
    
    REM Crea directory se non esiste
    echo 🔧 Controllo directory: %db_dir%
    if not exist "%db_dir%" (
        echo 🔧 Directory non esiste, creazione in corso...
        md "%db_dir%" 2>nul
        timeout /t 1 /nobreak >nul
        if exist "%db_dir%" (
            echo ✓ Creata directory: %db_dir%
        ) else (
            echo ❌ ERRORE: Impossibile creare directory %db_dir%
            echo 🔧 Debug: Tentativo creazione manuale...
            mkdir "%db_dir%"
            if exist "%db_dir%" (
                echo ✓ Directory creata con mkdir
            ) else (
                echo ❌ Fallito anche mkdir - problema di permessi
                pause
                exit /b 1
            )
        )
    ) else (
        echo ℹ Directory già esistente: %db_dir%
    )
    
    echo 📁 Destinazione: %db_file%
    echo 🔄 Avvio backup database...
    echo.
    
    set "db_name=Consulting-Full Database Backup Ver%new_db_ver%"
    sqlcmd -S "IT15\SQLEXPRESS" -Q "BACKUP DATABASE [Consulting] TO DISK = N'%db_file%' WITH FORMAT, INIT, NAME = N'%db_name%', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
    
    if !ERRORLEVEL! EQU 0 (
        echo.
        echo ✅ BACKUP DATABASE COMPLETATO!
        echo 📁 File: %db_file%
        echo 🏷 Versione: %new_db_ver%
    ) else (
        echo.
        echo ❌ ERRORE NEL BACKUP DATABASE
        echo Codice errore: !ERRORLEVEL!
        pause
        exit /b 1
    )
    echo.
)

REM ===== BACKUP CODICE =====
if "%do_code_backup%"=="true" (
    echo ==========================================
    echo 📂 BACKUP CODICE SORGENTE - VERSIONE %new_code_ver%
    echo ==========================================
    
    REM Crea directory se non esiste
    echo 🔧 Controllo directory: %code_dir%
    if not exist "%code_dir%" (
        echo 🔧 Directory non esiste, creazione in corso...
        md "%code_dir%" 2>nul
        timeout /t 1 /nobreak >nul
        if exist "%code_dir%" (
            echo ✓ Creata directory: %code_dir%
        ) else (
            echo ❌ ERRORE: Impossibile creare directory %code_dir%
            echo 🔧 Debug: Tentativo creazione manuale...
            mkdir "%code_dir%"
            if exist "%code_dir%" (
                echo ✓ Directory creata con mkdir
            ) else (
                echo ❌ Fallito anche mkdir - problema di permessi
                pause
                exit /b 1
            )
        )
    ) else (
        echo ℹ Directory già esistente: %code_dir%
    )
    
    echo 📁 Sorgente: C:\dev\prova
    echo 📁 Destinazione: %code_dir%
    echo 🔄 Avvio copia codice...
    echo.
    
    xcopy "C:\dev\prova" "%code_dir%" /E /I /H /Y
    
    if !ERRORLEVEL! EQU 0 (
        echo.
        echo ✅ BACKUP CODICE COMPLETATO!
        echo 📁 Directory: %code_dir%
        echo 🏷 Versione: %new_code_ver%
    ) else (
        echo.
        echo ❌ ERRORE NEL BACKUP CODICE
        echo Codice errore: !ERRORLEVEL!
        pause
        exit /b 1
    )
    echo.
)

echo ==========================================
echo 🎉 TUTTI I BACKUP COMPLETATI!
echo ==========================================
if "%do_db_backup%"=="true" (
    echo 💾 Database Ver%new_db_ver%: %db_file%
)
if "%do_code_backup%"=="true" (
    echo 📂 Codice Ver%new_code_ver%: %code_dir%
)
echo.
echo ✅ Operazione completata con successo!
echo.
pause