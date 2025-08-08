# ⚡ COMANDI RAPIDI - INSTALLAZIONE SERVER

## 🚀 INSTALLAZIONE AUTOMATICA

```batch
# 1. Eseguire come AMMINISTRATORE
install_server.bat

# 2. Configurare database SQL Server
# Aprire SCRIPTS_INSTALLAZIONE.sql in SQL Server Management Studio ed eseguire

# 3. Aggiornare stringa connessione
# Modificare: C:\inetpub\consulting.web\appsettings.Production.json

# 4. Testare installazione
check_server.bat
```

---

## 📁 PERCORSI IMPORTANTI

| Elemento | Percorso |
|----------|----------|
| **Applicazione** | `C:\inetpub\consulting.web\` |
| **Log** | `C:\inetpub\logs\consulting\` |
| **Configurazione** | `C:\inetpub\consulting.web\appsettings.Production.json` |
| **Database** | `ConsultingGroupDB` |
| **Sito IIS** | `ConsultingGroup` |

---

## 🔧 COMANDI IIS

```batch
# Creare sito
%windir%\system32\inetsrv\appcmd.exe add site /name:"ConsultingGroup" /physicalPath:"C:\inetpub\consulting.web" /bindings:http/*:80:

# Configurare pool
%windir%\system32\inetsrv\appcmd.exe set apppool "ConsultingGroup" /managedRuntimeVersion:

# Avviare sito
%windir%\system32\inetsrv\appcmd.exe start site "ConsultingGroup"

# Fermare sito
%windir%\system32\inetsrv\appcmd.exe stop site "ConsultingGroup"

# Rimuovere sito
%windir%\system32\inetsrv\appcmd.exe delete site "ConsultingGroup"
```

---

## 🗄️ COMANDI DATABASE

```sql
-- Verifica database
USE ConsultingGroupDB
SELECT COUNT(*) FROM AspNetUsers  -- Deve restituire almeno 3 utenti

-- Reset password admin (se necessario)
UPDATE AspNetUsers 
SET PasswordHash = 'AQAAAAEAACcQAAAAEGXj8lGQ9h...' -- Hash per "123456"
WHERE UserName = 'admin'

-- Verifica connessioni attive
SELECT 
    DB_NAME() as DatabaseName,
    COUNT(*) as ActiveConnections 
FROM sys.dm_exec_sessions 
WHERE database_id = DB_ID()
```

---

## 🔐 UTENTI PREDEFINITI

| Username | Password | Ruolo | Accesso |
|----------|----------|--------|---------|
| `admin` | `123456` | Administrator | Completo + impostazioni |
| `senior` | `123456` | UserSenior | Dati riservati + reports |
| `user` | `123456` | User | Solo dati generali |

---

## 📊 VERIFICA RAPIDA

```batch
# Test completo
check_server.bat

# Test solo applicazione
curl http://localhost

# Test database
sqlcmd -S localhost -d ConsultingGroupDB -Q "SELECT COUNT(*) FROM AspNetUsers"
```

---

## 🆘 TROUBLESHOOTING RAPIDO

### Errore 500.30
```batch
# Verifica .NET Runtime
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App 9."

# Reset permessi
icacls "C:\inetpub\consulting.web" /grant "IIS_IUSRS:(OI)(CI)RX" /T
```

### Errore Database
```batch
# Test connessione
sqlcmd -S localhost -U consulting_user -P Password_Sicura_123! -d ConsultingGroupDB -Q "SELECT @@VERSION"
```

### Reset completo
```batch
# Disinstallazione
uninstall_server.bat

# Reinstallazione
install_server.bat
```

---

## 📞 URL IMPORTANTI

- **Applicazione**: http://localhost
- **Login**: http://localhost/Account/Login
- **Dashboard Admin**: http://localhost/Admin
- **Gestione IIS**: http://localhost:8080 (se abilitato)

---

## 🔄 MANUTENZIONE

```batch
# Backup database
sqlcmd -S localhost -Q "BACKUP DATABASE ConsultingGroupDB TO DISK = 'C:\Backup\ConsultingGroup.bak'"

# Restart applicazione
%windir%\system32\inetsrv\appcmd.exe recycle apppool "ConsultingGroup"

# Clear log
del "C:\inetpub\logs\consulting\*.log"

# Aggiorna applicazione (nuovo deploy)
xcopy "publish\*" "C:\inetpub\consulting.web\" /E /Y /I
%windir%\system32\inetsrv\appcmd.exe recycle apppool "ConsultingGroup"
```
