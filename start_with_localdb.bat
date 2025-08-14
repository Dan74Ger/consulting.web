@echo off
echo Avvio Applicazione con LocalDB
echo ===============================
echo.

REM Esegui lo script PowerShell
powershell -ExecutionPolicy Bypass -File "start_with_localdb.ps1"

echo.
echo Script completato!
pause 