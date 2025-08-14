@echo off
REM Script batch per commit automatici
REM Uso: auto_commit.bat [messaggio personalizzato]

echo.
echo ========================================
echo    COMMIT AUTOMATICO SU GITHUB
echo ========================================
echo.

cd /d "C:\dev\prova"

if "%~1"=="" (
    powershell -ExecutionPolicy Bypass -File "auto_commit.ps1"
) else (
    powershell -ExecutionPolicy Bypass -File "auto_commit.ps1" -CommitMessage "%*"
)

echo.
echo Premi un tasto per continuare...
pause >nul
