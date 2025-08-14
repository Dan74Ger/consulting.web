@echo off
echo ========================================
echo BACKUP COMPLETO CONSULTING GROUP
echo ========================================
echo.
echo Data: %date% %time%
echo.

REM Esegui lo script PowerShell per backup completo
powershell -ExecutionPolicy Bypass -File "BACKUP_COMPLETO.ps1"

echo.
echo Script completato!
pause 