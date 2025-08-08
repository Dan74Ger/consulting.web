@echo off
echo.
echo ============================================
echo VERIFICA URL SITO - CONSULTING GROUP SRL
echo ============================================
echo.

echo [CONFIGURAZIONE IIS]
echo.
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" 2>nul
if %errorLevel% neq 0 (
    echo ✗ Sito IIS non configurato
    echo   Eseguire prima: install_server.bat
    goto :end
)

echo.
echo [BINDING URL]
echo.
for /f "tokens=*" %%i in ('%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" ^| findstr "bindings"') do (
    echo %%i
    echo.
    echo Il tuo sito è configurato su:
    echo.
    echo ✅ http://localhost
    if "%%i" == "*:80:" (
        echo ✅ http://localhost:80
    )
    if "%%i" == "*:443:" (
        echo ✅ https://localhost:443
    )
)

echo.
echo [TEST CONNESSIONE]
echo.
echo Testando connessione HTTP...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://localhost' -UseBasicParsing -TimeoutSec 5; Write-Host '✅ SITO RAGGIUNGIBILE: http://localhost' -ForegroundColor Green; Write-Host '   Status Code:' $response.StatusCode } catch { Write-Host '❌ SITO NON RAGGIUNGIBILE: http://localhost' -ForegroundColor Red; Write-Host '   Errore:' $_.Exception.Message }"

echo.
echo ============================================
echo RIEPILOGO URL
echo ============================================
echo.
echo 🌐 URL PRINCIPALE: http://localhost
echo 📱 URL MOBILE: http://localhost (stesso)
echo 🔐 LOGIN PAGE: http://localhost/Account/Login
echo 👨‍💼 ADMIN AREA: http://localhost/Admin
echo.
echo 👤 CREDENZIALI TEST:
echo    - Admin: admin / 123456
echo    - Senior: senior / 123456  
echo    - User: user / 123456
echo.

:end
pause
