@echo off
echo ========================================
echo BACKUP SITO CONSULTING GROUP
echo ========================================
echo.
echo Data: %date% %time%
echo.

REM Esegui lo script PowerShell per backup sito
powershell -ExecutionPolicy Bypass -File "BACKUP_COMPLETO_SITO.ps1"

echo.
echo Script completato!
pause 