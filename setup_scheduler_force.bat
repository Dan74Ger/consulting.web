@echo off
REM Script per configurare Task Scheduler forzando ExecutionPolicy

echo.
echo ========================================
echo   SETUP COMMIT AUTOMATICI - FORZATO
echo ========================================
echo.

echo Questo script configurerà commit automatici giornalieri alle 18:00
echo.

REM Forza l'esecuzione del PowerShell script
powershell -ExecutionPolicy Bypass -Command "& {Set-ExecutionPolicy Bypass -Scope Process -Force; C:\dev\prova\setup_scheduler.ps1}"

echo.
echo Operazione completata!
pause
