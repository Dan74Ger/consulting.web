@echo off
echo ========================================
echo BACKUP DATABASE CONSULTING - VERSIONE 27
echo ========================================
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo Destinazione: C:\dev\vprova_backup_ver27_database\
echo.

echo Creazione directory se non esiste...
if not exist "C:\dev\vprova_backup_ver27_database\" mkdir "C:\dev\vprova_backup_ver27_database\"

echo.
echo Avvio backup del database...
sqlcmd -S "IT15\SQLEXPRESS" -Q "BACKUP DATABASE [Consulting] TO DISK = N'C:\dev\vprova_backup_ver27_database\Consulting_backup_ver27.bak' WITH FORMAT, INIT, NAME = N'Consulting-Full Database Backup Ver27', SKIP, NOREWIND, NOUNLOAD, STATS = 10"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ BACKUP COMPLETATO CON SUCCESSO!
    echo ✓ File: C:\dev\vprova_backup_ver27_database\Consulting_backup_ver27.bak
    echo ✓ Versione: 27 - Include vincoli CHECK aggiornati per periodicità
) else (
    echo.
    echo ✗ ERRORE NEL BACKUP
    echo Codice errore: %ERRORLEVEL%
)

echo.
pause

