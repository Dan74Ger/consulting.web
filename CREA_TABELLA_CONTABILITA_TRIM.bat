@echo off
echo =====================================================
echo CREAZIONE TABELLA CONTABILITA INTERNA TRIMESTRALE
echo =====================================================
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo =====================================================
echo.

echo Esecuzione script SQL...
sqlcmd -S "IT15\SQLEXPRESS" -d "Consulting" -i "create_contabilita_interna_trimestrale.sql"

if %ERRORLEVEL% equ 0 (
    echo.
    echo ✅ TABELLA CREATA CON SUCCESSO!
    echo.
) else (
    echo.
    echo ❌ ERRORE NELLA CREAZIONE DELLA TABELLA
    echo Codice errore: %ERRORLEVEL%
    echo.
)

pause
