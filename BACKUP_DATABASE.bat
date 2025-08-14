@echo off
echo ========================================
echo BACKUP DATABASE CONSULTING GROUP
echo ========================================
echo.
echo Data: %date% %time%
echo.

REM Esegui lo script PowerShell per backup database
powershell -ExecutionPolicy Bypass -File "BACKUP_DATABASE.ps1"

echo.
echo Script completato!
pause 