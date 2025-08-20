@echo off
echo =====================================================
echo AGGIUNTA CAMPI IMPORTO CREDITO/DEBITO
echo =====================================================
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo Tabella: contabilita_interna_trimestrale
echo =====================================================
echo.

echo Esecuzione script SQL per aggiungere i nuovi campi...
sqlcmd -S "IT15\SQLEXPRESS" -d "Consulting" -i "add_importi_credito_debito.sql"

if %ERRORLEVEL% equ 0 (
    echo.
    echo ✅ CAMPI IMPORTO CREDITO/DEBITO AGGIUNTI CON SUCCESSO!
    echo.
    echo Nuovi campi aggiunti per ogni trimestre:
    echo   - [trimestre]_importo_credito (DECIMAL 10,2)
    echo   - [trimestre]_importo_debito (DECIMAL 10,2)
    echo.
) else (
    echo.
    echo ❌ ERRORE NELL'AGGIUNTA DEI CAMPI
    echo Codice errore: %ERRORLEVEL%
    echo.
)

pause
