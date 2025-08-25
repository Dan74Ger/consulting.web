@echo off
echo ========================================
echo   RINOMINARE ENTRAMBE LE COLONNE
echo        CONTABILITA' DATABASE
echo ========================================
echo.
echo Questo script rinomina:
echo 1. 'Contabilita' ^> 'contabilita_interna_trimestrale' 
echo 2. 'periodo_contabilita' ^> 'contabilita_interna_mensile'
echo.
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo.
pause

echo Esecuzione script SQL...
sqlcmd -S IT15\SQLEXPRESS -d Consulting -i rename_both_contabilita_columns.sql -o rename_both_contabilita_output.log

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERRORE durante l'esecuzione dello script!
    echo Controlla il file rename_both_contabilita_output.log per i dettagli
    pause
    exit /b 1
) else (
    echo.
    echo Script eseguito con successo!
    echo Log salvato in: rename_both_contabilita_output.log
)

echo.
pause
