@echo off
echo.
echo ============================================
echo TEST http://consulting.local
echo ============================================
echo.

echo [VERIFICA CONFIGURAZIONE]
echo.

REM Verifica binding IIS
echo Binding IIS:
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup" | findstr "bindings"

echo.
echo File hosts:
findstr "consulting.local" %windir%\System32\drivers\etc\hosts

echo.
echo [PING TEST]
echo.
ping consulting.local -n 1

echo.
echo [HTTP TEST]
echo.
echo Testando http://consulting.local...
powershell -Command "try { $response = Invoke-WebRequest -Uri 'http://consulting.local' -UseBasicParsing -TimeoutSec 10; Write-Host '✅ SITO RAGGIUNGIBILE: http://consulting.local' -ForegroundColor Green; Write-Host '   Status Code:' $response.StatusCode; Write-Host '   Content Length:' $response.Content.Length } catch { Write-Host '❌ SITO NON RAGGIUNGIBILE: http://consulting.local' -ForegroundColor Red; Write-Host '   Errore:' $_.Exception.Message }"

echo.
echo ============================================
echo RISULTATO TEST
echo ============================================
echo.
echo Se tutto è OK, apri browser e vai su:
echo.
echo 🌐 http://consulting.local
echo 🔐 http://consulting.local/Account/Login
echo.
echo Credenziali di accesso:
echo 👤 admin / 123456
echo 👤 senior / 123456
echo 👤 user / 123456
echo.

pause
