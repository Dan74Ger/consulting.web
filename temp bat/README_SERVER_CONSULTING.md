# 🏢 GESTIONE STUDIO - CONSULTING GROUP SRL
## 📦 INSTALLAZIONE SERVER PERSONALIZZATA

---

## ⚙️ **CONFIGURAZIONE SERVER**

| Parametro | Valore |
|-----------|--------|
| **Database Server** | `SRV-dc\SQLEXPRESS` |
| **Database Name** | `Consulting` |
| **Autenticazione** | Windows Authentication |
| **URL Sito** | `http://consulting.local` |
| **Cartella Web** | `C:\inetpub\consulting.web` |

---

## 🚀 **INSTALLAZIONE AUTOMATICA**

### **STEP 1: Copiare files sul server**
Copiare tutti questi files nella cartella del server:
- ✅ `INSTALL_SERVER_CONSULTING.bat`
- ✅ `CHECK_SERVER_CONSULTING.bat`
- ✅ `UNINSTALL_SERVER_CONSULTING.bat`
- ✅ Cartella `publish` (completa)

### **STEP 2: Eseguire installazione**
```batch
# Eseguire come AMMINISTRATORE
INSTALL_SERVER_CONSULTING.bat
```

### **STEP 3: Verificare installazione**
```batch
CHECK_SERVER_CONSULTING.bat
```

### **STEP 4: Accesso al sito**
```
http://consulting.local
```

---

## 📋 **COSA FA L'INSTALLAZIONE AUTOMATICA**

Lo script `INSTALL_SERVER_CONSULTING.bat` esegue automaticamente:

### ✅ **Verifica Prerequisiti**
- IIS installato e funzionante
- .NET 9.0 ASP.NET Core Runtime
- Connessione a `SRV-dc\SQLEXPRESS`

### ✅ **Configurazione Database**
- Verifica/crea database `Consulting`
- Testa autenticazione Windows
- Configura stringa connessione

### ✅ **Installazione Applicazione**
- Crea cartelle `C:\inetpub\consulting.web`
- Copia tutti i files dalla cartella `publish`
- Imposta permessi corretti per IIS

### ✅ **Configurazione IIS**
- Crea sito "ConsultingGroup"
- Configura binding per `consulting.local`
- Imposta pool applicazioni .NET Core
- Configura variabili ambiente

### ✅ **Configurazione DNS Locale**
- Modifica file hosts Windows
- Aggiunge `127.0.0.1 consulting.local`
- Pulisce cache DNS

### ✅ **Migration Database**
- Esegue l'applicazione per creare tabelle
- Inserisce utenti predefiniti
- Configura ruoli e permessi

---

## 🎯 **RISULTATO FINALE**

Dopo l'installazione completata:

### **🌐 URL Accesso**
```
http://consulting.local
```

### **👥 Utenti Predefiniti**
| Username | Password | Ruolo | Accesso |
|----------|----------|--------|---------|
| `admin` | `123456` | **Administrator** | Completo + impostazioni |
| `senior` | `123456` | **UserSenior** | Dati riservati + reports |
| `user` | `123456` | **User** | Solo dati generali |

### **📊 Sezioni Disponibili**
- 🏦 **Dati Riservati**: Banche, Carte, Utenze, Mail
- 📁 **Dati Generali**: Cancelleria, PC, Entratel, TeamSystem
- 👥 **Gestione Utenti**: Solo Administrator
- 📊 **Reports**: Administrator e Senior

---

## 🔧 **MANUTENZIONE**

### **Verifica Sistema**
```batch
CHECK_SERVER_CONSULTING.bat
```

### **Restart Applicazione**
```batch
%windir%\system32\inetsrv\appcmd.exe recycle apppool "ConsultingGroup"
```

### **Backup Database**
```sql
-- In SQL Server Management Studio
BACKUP DATABASE Consulting 
TO DISK = 'C:\Backup\Consulting_YYYYMMDD.bak'
```

### **Update Applicazione**
1. Fare backup del database
2. Copiare nuovi files in `C:\inetpub\consulting.web`
3. Restart pool applicazioni

---

## 🆘 **RISOLUZIONE PROBLEMI**

### **❌ Errore connessione database**
```batch
# Test manuale connessione
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -Q "SELECT @@VERSION"
```

### **❌ Sito non raggiungibile**
```batch
# Verifica binding IIS
%windir%\system32\inetsrv\appcmd.exe list site "ConsultingGroup"

# Verifica file hosts
findstr "consulting.local" %windir%\System32\drivers\etc\hosts
```

### **❌ Errore 500.30**
```batch
# Verifica .NET Runtime
dotnet --list-runtimes | findstr "Microsoft.AspNetCore.App 9."
```

### **❌ Login non funziona**
```sql
-- Verifica utenti nel database
USE Consulting
SELECT UserName, Email FROM AspNetUsers
```

---

## 🗑️ **DISINSTALLAZIONE**

Per rimuovere completamente l'applicazione:

```batch
# Eseguire come AMMINISTRATORE
UNINSTALL_SERVER_CONSULTING.bat
```

Questo rimuoverà:
- ✅ Sito IIS
- ✅ Pool applicazioni
- ✅ Entry file hosts
- ✅ Files applicazione (opzionale)
- ✅ Database (opzionale)

---

## 📞 **SUPPORTO**

### **Files di Log**
- Applicazione: `C:\inetpub\logs\consulting\`
- IIS: `C:\inetpub\logs\LogFiles\`
- Windows Event Viewer: Application logs

### **Comandi Utili**
```batch
# Status servizi
sc query W3SVC
sc query "SQL Server (SQLEXPRESS)"

# Test database
sqlcmd -S "SRV-dc\SQLEXPRESS" -E -d "Consulting" -Q "SELECT COUNT(*) FROM AspNetUsers"

# Test HTTP
curl http://consulting.local
```

---

## ✅ **CHECKLIST POST-INSTALLAZIONE**

- ☐ Sito raggiungibile su `http://consulting.local`
- ☐ Login admin/123456 funzionante
- ☐ Accesso dati riservati (admin/senior)
- ☐ Accesso dati generali (tutti)
- ☐ CRUD completo funzionante
- ☐ Permessi ruoli corretti
- ☐ **Cambio password predefinite**
- ☐ Backup database configurato

---

**🎉 SISTEMA PRONTO PER LA PRODUZIONE!**

**Database**: `SRV-dc\SQLEXPRESS\Consulting`  
**URL**: `http://consulting.local`  
**Login**: `admin/123456`
