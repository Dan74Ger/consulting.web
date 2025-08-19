@echo off
echo Aggiornamento vincolo CHECK su tabella proforma_generate...
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo.

sqlcmd -S "IT15\SQLEXPRESS" -d "Consulting" -E -i "ADD_SEMESTRALE_PROFORMA_VINCOLO.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Vincolo proforma_generate aggiornato con successo!
    echo ✓ Ora 'semestrale' e 'bimestrale' sono consentiti nel campo tipo_proforma
) else (
    echo.
    echo ✗ Errore nell'aggiornamento del vincolo
    echo Codice errore: %ERRORLEVEL%
)

echo.
pause
