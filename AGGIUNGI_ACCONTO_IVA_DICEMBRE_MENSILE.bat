@echo off
echo Aggiunta campo Acconto IVA a dicembre nella contabilita mensile...
sqlcmd -S IT15\SQLEXPRESS -d Consulting -i add_acconto_iva_dicembre_mensile.sql
if %errorlevel% neq 0 (
    echo Errore durante l'aggiunta del campo Acconto IVA a dicembre
    pause
    exit /b 1
)
echo Campo Acconto IVA aggiunto con successo a dicembre!
pause
