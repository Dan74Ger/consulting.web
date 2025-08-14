@echo off
echo ========================================
echo APPLICAZIONE MODIFICHE AL DATABASE
echo ========================================
echo.
echo Questo script aggiungera la colonna CanAccessAreaAmministrativa
echo alla tabella UserPermissions nel database Consulting.
echo.

REM Verifica se SQL Server è accessibile
echo Verifica connessione al database...
sqlcmd -S "PCESTERNO-D\SQLEXPRESS" -d "Consulting" -Q "SELECT GETDATE() as CurrentTime" -E
if %ERRORLEVEL% NEQ 0 (
    echo ERRORE: Impossibile connettersi al database!
    echo Verifica che SQL Server sia in esecuzione e accessibile.
    pause
    exit /b 1
)

echo.
echo Connessione al database riuscita!
echo.
echo Applicazione delle modifiche...

REM Esegui lo script SQL
sqlcmd -S "PCESTERNO-D\SQLEXPRESS" -d "Consulting" -i "ADD_AREA_AMMINISTRATIVA_COLUMN.sql" -E

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo MODIFICHE APPLICATE CON SUCCESSO!
    echo ========================================
    echo.
    echo La colonna CanAccessAreaAmministrativa e' stata aggiunta
    echo alla tabella UserPermissions.
    echo.
    echo I permessi per gli utenti Senior sono stati aggiornati
    echo automaticamente.
    echo.
) else (
    echo.
    echo ERRORE durante l'applicazione delle modifiche!
    echo Controlla i messaggi di errore sopra.
    echo.
)

echo Premere un tasto per continuare...
pause > nul
