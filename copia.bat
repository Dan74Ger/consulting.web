@echo off
setlocal enabledelayedexpansion
chcp 65001 >nul

echo ============================================
echo   BACKUP AUTOMATICO DATABASE E CODICE
echo ============================================
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo Codice: C:\dev\prova
echo ============================================
echo.

REM ===== RILEVA VERSIONE AUTOMATICAMENTE =====
set "max_ver=0"
for /d %%i in ("C:\dev\vprova_backup_ver*_database") do (
    set "folder=%%~ni"
    set "folder=!folder:vprova_backup_ver=!"
    set "folder=!folder:_database=!"
    if !folder! gtr !max_ver! set "max_ver=!folder!"
)

REM Se non trova directory esistenti, parte da 0
if !max_ver! equ 0 (
    for /d %%i in ("C:\dev\vprova_backup_ver*_codice") do (
        set "folder=%%~ni"
        set "folder=!folder:vprova_backup_ver=!"
        set "folder=!folder:_codice=!"
        if !folder! gtr !max_ver! set "max_ver=!folder!"
    )
)

set /a "new_ver=max_ver+1"

set "db_dir=C:\dev\vprova_backup_ver!new_ver!_database"
set "code_dir=C:\dev\vprova_backup_ver!new_ver!_codice"
set "db_file=!db_dir!\Consulting_backup_ver!new_ver!.bak"

echo NUOVA VERSIONE: !new_ver!
echo.
echo DESTINAZIONI:
echo   Database: !db_file!
echo   Codice:   !code_dir!
echo.

REM ===== CHIEDI COSA FARE =====
echo Cosa vuoi fare?
echo [1] Solo DATABASE
echo [2] Solo CODICE
echo [3] ENTRAMBI
echo [4] ANNULLA
echo.
set /p "scelta=Scegli (1-4): "

if "!scelta!"=="4" (
    echo.
    echo Operazione annullata.
    pause
    exit /b 0
)

if "!scelta!"=="1" set "do_db=true" & set "do_code=false"
if "!scelta!"=="2" set "do_db=false" & set "do_code=true"
if "!scelta!"=="3" set "do_db=true" & set "do_code=true"

if not "!scelta!"=="1" if not "!scelta!"=="2" if not "!scelta!"=="3" (
    echo.
    echo Scelta non valida. Uscita.
    pause
    exit /b 1
)

echo.
echo ==========================================
echo           AVVIO BACKUP VER !new_ver!
echo ==========================================

REM ===== BACKUP DATABASE =====
if "!do_db!"=="true" (
    echo.
    echo [DATABASE] Creazione directory...
    if not exist "!db_dir!" (
        md "!db_dir!" 2>nul
        if exist "!db_dir!" (
            echo [DATABASE] Directory creata: !db_dir!
        ) else (
            echo [DATABASE] ERRORE: Impossibile creare directory
            pause
            exit /b 1
        )
    ) else (
        echo [DATABASE] Directory gia' esistente: !db_dir!
    )
    
    echo [DATABASE] Avvio backup su IT15\SQLEXPRESS...
    echo.
    
    sqlcmd -S "IT15\SQLEXPRESS" -Q "BACKUP DATABASE [Consulting] TO DISK = N'!db_file!' WITH FORMAT, INIT, NAME = N'Consulting-Backup-Ver!new_ver!', SKIP, NOREWIND, NOUNLOAD, STATS = 10"
    
    if !ERRORLEVEL! equ 0 (
        echo.
        echo [DATABASE] COMPLETATO! File: !db_file!
    ) else (
        echo.
        echo [DATABASE] ERRORE durante il backup
        pause
        exit /b 1
    )
)

REM ===== BACKUP CODICE =====
if "!do_code!"=="true" (
    echo.
    echo [CODICE] Creazione directory...
    if not exist "!code_dir!" (
        md "!code_dir!" 2>nul
        if exist "!code_dir!" (
            echo [CODICE] Directory creata: !code_dir!
        ) else (
            echo [CODICE] ERRORE: Impossibile creare directory
            pause
            exit /b 1
        )
    ) else (
        echo [CODICE] Directory gia' esistente: !code_dir!
    )
    
    echo [CODICE] Copia da C:\dev\prova...
    echo.
    
    xcopy "C:\dev\prova" "!code_dir!" /E /I /H /Y /Q
    
    if !ERRORLEVEL! equ 0 (
        echo.
        echo [CODICE] COMPLETATO! Directory: !code_dir!
    ) else (
        echo.
        echo [CODICE] ERRORE durante la copia
        pause
        exit /b 1
    )
)

echo.
echo ==========================================
echo            BACKUP COMPLETATO!
echo ==========================================
echo Versione: !new_ver!
if "!do_db!"=="true" echo Database: !db_file!
if "!do_code!"=="true" echo Codice: !code_dir!
echo.
echo Tutto fatto con successo!
echo.
pause
