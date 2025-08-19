@echo off
echo Aggiornamento vincolo CHECK per includere 'bimestrale'...
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo.

sqlcmd -S "IT15\SQLEXPRESS" -d "Consulting" -E -i "ADD_BIMESTRALE_VINCOLO.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Vincolo aggiornato con successo!
    echo ✓ Ora 'bimestrale' è consentito nel campo proforma_tipo
) else (
    echo.
    echo ✗ Errore nell'aggiornamento del vincolo
    echo Codice errore: %ERRORLEVEL%
)

echo.
pause
