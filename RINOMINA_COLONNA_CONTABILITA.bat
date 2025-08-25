@echo off
echo ========================================
echo   RINOMINARE COLONNA CONTABILITA
echo ========================================
echo.
echo Questo script rinomina la colonna 'Contabilita' 
echo in 'ContabilitaInternaTrimestrale' nella tabella clienti
echo.
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo.
pause

echo Esecuzione script SQL...
sqlcmd -S IT15\SQLEXPRESS -d Consulting -i rename_contabilita_column.sql -o rename_contabilita_output.log

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERRORE durante l'esecuzione dello script!
    echo Controlla il file rename_contabilita_output.log per i dettagli
    pause
    exit /b 1
) else (
    echo.
    echo Script eseguito con successo!
    echo Log salvato in: rename_contabilita_output.log
)

echo.
pause
