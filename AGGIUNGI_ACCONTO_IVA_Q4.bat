@echo off
echo Aggiunta campo Acconto IVA al quarto trimestre...
sqlcmd -S IT15\SQLEXPRESS -d Consulting -i add_acconto_iva_q4.sql
if %errorlevel% neq 0 (
    echo Errore durante l'aggiunta del campo Acconto IVA
    pause
    exit /b 1
)
echo Campo Acconto IVA aggiunto con successo!
pause
