@echo off
echo ===================================================
echo AGGIUNTA CAMPI: IVA da versare con interessi
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo ===================================================

sqlcmd -S "IT15\SQLEXPRESS" -d "Consulting" -i "add_iva_versare_interessi.sql"

if %errorlevel% equ 0 (
    echo.
    echo ✅ Campi aggiunti con successo!
) else (
    echo.
    echo ❌ Errore durante l'esecuzione dello script
)

pause
