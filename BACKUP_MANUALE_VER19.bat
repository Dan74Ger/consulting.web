@echo off
echo === BACKUP MANUALE CONSULTING GROUP - VER19 ===
echo.

REM Crea le cartelle di backup
echo Creazione cartelle di backup...
if not exist "D:\dev\vprova_backup_ver19_codice" mkdir "D:\dev\vprova_backup_ver19_codice"
if not exist "D:\dev\vprova_backup_ver19_database" mkdir "D:\dev\vprova_backup_ver19_database"

echo.
echo 1. BACKUP CODICE SORGENTE...
robocopy "D:\dev\prova" "D:\dev\vprova_backup_ver19_codice" /E /XD bin obj packages .vs .git node_modules "temp bat" "temp db" /R:3 /W:1

echo.
echo 2. BACKUP DATABASE...
sqlcmd -S "PCESTERNO-D\SQLEXPRESS" -E -Q "BACKUP DATABASE [Consulting] TO DISK = 'D:\dev\vprova_backup_ver19_database\Consulting_VER19_%date:~-4,4%%date:~-10,2%%date:~-7,2%_%time:~0,2%%time:~3,2%%time:~6,2%.bak' WITH FORMAT, NAME = 'Consulting Database Backup VER19'"

echo.
echo 3. COPIA SCRIPT DI RIPRISTINO...
copy "RIPRISTINA_SU_ALTRO_PC.ps1" "D:\dev\vprova_backup_ver19_database\" 2>nul

echo.
echo === BACKUP COMPLETATO ===
echo Codice: D:\dev\vprova_backup_ver19_codice
echo Database: D:\dev\vprova_backup_ver19_database
echo.
pause
