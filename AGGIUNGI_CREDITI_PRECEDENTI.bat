@echo off
echo =====================================================
echo AGGIUNTA CAMPI CREDITI PRECEDENTI
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo =====================================================
echo.

echo Esecuzione script SQL...
sqlcmd -S "IT15\SQLEXPRESS" -d "Consulting" -i "add_crediti_precedenti.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✅ SCRIPT ESEGUITO CON SUCCESSO!
    echo ✅ Aggiunte colonne per crediti precedenti nella tabella contabilita_interna_trimestrale
) else (
    echo.
    echo ❌ ERRORE durante l'esecuzione dello script!
    echo ❌ Controllare la connessione al database
)

echo.
pause
