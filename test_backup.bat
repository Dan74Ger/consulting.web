@echo off
echo ===== TEST CREAZIONE DIRECTORY =====

REM Test 1: Creazione semplice
echo Test 1: Creazione directory semplice
set "test_dir1=C:\dev\test_backup_simple"
echo Tentativo creazione: %test_dir1%
md "%test_dir1%" 2>nul
if exist "%test_dir1%" (
    echo ✓ SUCCESS: Directory creata
    rd "%test_dir1%" 2>nul
) else (
    echo ❌ FAIL: Impossibile creare directory
)
echo.

REM Test 2: Test permessi in C:\dev
echo Test 2: Test permessi C:\dev
echo Controllo se C:\dev esiste...
if exist "C:\dev\" (
    echo ✓ C:\dev esiste
) else (
    echo ❌ C:\dev non esiste!
    pause
    exit /b 1
)
echo.

REM Test 3: Creazione directory con nome versione
echo Test 3: Directory con nome backup
set "test_dir2=C:\dev\vprova_backup_ver1_database"
echo Tentativo creazione: %test_dir2%
md "%test_dir2%" 2>nul
if exist "%test_dir2%" (
    echo ✓ SUCCESS: Directory backup creata
    echo Contenuto C:\dev dopo creazione:
    dir C:\dev\vprova_backup_ver* 2>nul
    echo.
    echo Rimozione directory test...
    rd "%test_dir2%" 2>nul
) else (
    echo ❌ FAIL: Impossibile creare directory backup
)
echo.

echo Test completato.
pause

