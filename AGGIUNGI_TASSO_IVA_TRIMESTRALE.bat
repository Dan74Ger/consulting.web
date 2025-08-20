@echo off
echo ===================================================
echo AGGIUNTA CAMPO: Tasso per IVA Trimestrale
echo Server: IT15\SQLEXPRESS
echo Database: Consulting
echo ===================================================

sqlcmd -S "IT15\SQLEXPRESS" -d "Consulting" -i "add_tasso_iva_trimestrale.sql"

if %errorlevel% equ 0 (
    echo.
    echo ✅ Campo aggiunto con successo!
) else (
    echo.
    echo ❌ Errore durante l'esecuzione dello script
)

pause
