@echo off
echo Avvio Applicazione con SQL Server
echo =================================
echo.

REM Esegui lo script PowerShell
powershell -ExecutionPolicy Bypass -File "start_with_sqlserver.ps1"

echo.
echo Script completato!
pause 