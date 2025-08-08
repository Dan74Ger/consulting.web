@echo off
echo ============================================
echo COPIA APPLICAZIONE AL SERVER
echo ============================================

echo [1/4] Creazione cartella destinazione...
if not exist "C:\inetpub" mkdir "C:\inetpub"
if not exist "C:\inetpub\consulting.web" mkdir "C:\inetpub\consulting.web"

echo [2/4] Copia file applicazione...
xcopy "publish\*" "C:\inetpub\consulting.web\" /E /Y /I

echo [3/4] Copia file configurazione server...
copy "appsettings.Server.json" "C:\inetpub\consulting.web\appsettings.Production.json"

echo [4/4] Copia script installazione...
copy "SCRIPTS_INSTALLAZIONE.sql" "C:\inetpub\consulting.web\"
copy "INSTALL_SERVER_CONSULTING.bat" "C:\inetpub\consulting.web\"
copy "CHECK_SERVER_CONSULTING.bat" "C:\inetpub\consulting.web\"
copy "CONFIGURE_HOSTS.bat" "C:\inetpub\consulting.web\"
copy "DATABASE_MIGRATION.bat" "C:\inetpub\consulting.web\"

echo.
echo ============================================
echo APPLICAZIONE COPIATA CORRETTAMENTE!
echo ============================================
echo.
echo PERCORSO: C:\inetpub\consulting.web
echo.
echo PROSSIMI PASSI:
echo 1. Installare IIS (se non fatto)
echo 2. Scaricare .NET 9.0 Hosting Bundle
echo 3. Eseguire: INSTALL_SERVER_CONSULTING.bat
echo.
pause
