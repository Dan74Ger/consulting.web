# 🏢 GESTIONE STUDIO - CONSULTING GROUP SRL
## 📦 PACCHETTO COMPLETO INSTALLAZIONE SERVER

---

## 🎯 **PROCEDURA INSTALLAZIONE NELLA CARTELLA C:\inetpub\consulting.web**

### ⚡ **INSTALLAZIONE RAPIDA (3 STEP)**

1. **📋 ESEGUIRE COME AMMINISTRATORE:**
   ```batch
   install_server.bat
   ```

2. **🗄️ CONFIGURARE DATABASE:**
   - Aprire `SCRIPTS_INSTALLAZIONE.sql` in SQL Server Management Studio
   - Eseguire tutto lo script

3. **🔧 AGGIORNARE CONFIGURAZIONE:**
   - Modificare stringa connessione in: `C:\inetpub\consulting.web\appsettings.Production.json`
   - Sostituire `localhost` con nome del server SQL

4. **✅ VERIFICARE INSTALLAZIONE:**
   ```batch
   check_server.bat
   ```

5. **🌐 TESTARE ACCESSO:**
   - Aprire browser: `http://localhost`
   - Login: `admin` / `123456`

---

## 📁 **FILES INSTALLAZIONE INCLUSI**

| File | Descrizione |
|------|-------------|
| 📘 `INSTALLAZIONE_SERVER.md` | **Guida completa dettagliata** |
| ⚡ `install_server.bat` | **Script installazione automatica** |
| 🗄️ `SCRIPTS_INSTALLAZIONE.sql` | **Script configurazione database** |
| ⚙️ `appsettings.Server.json` | **Configurazione server ottimizzata** |
| ✅ `check_server.bat` | **Script verifica installazione** |
| 🗑️ `uninstall_server.bat` | **Script disinstallazione** |
| 📋 `CHECKLIST_INSTALLAZIONE.md` | **Lista controllo step-by-step** |
| ⚡ `COMANDI_RAPIDI.md` | **Comandi rapidi e troubleshooting** |

---

## 🎯 **CARTELLA DESTINAZIONE SERVER**

```
C:\inetpub\consulting.web\          # ← CARTELLA PRINCIPALE
├── ConsultingGroup.dll             # Assembly applicazione
├── web.config                      # Configurazione IIS
├── appsettings.Production.json     # Configurazione produzione
├── Views\                          # Tutte le 65 view complete
│   ├── Home\                       # ✅ Index.cshtml, Error.cshtml
│   ├── Account\                    # ✅ Login, AccessDenied
│   ├── Admin\                      # ✅ Dashboard amministratore
│   ├── DatiUtenza\                 # ✅ 17 view CRUD complete
│   ├── DatiUtenzaExtra\            # ✅ 21 view cancelleria completa
│   ├── GestioneClienti\            # ✅ Gestione clienti
│   ├── Senior\                     # ✅ Dashboard senior
│   ├── User\                       # ✅ Dashboard utente
│   ├── UserManagement\             # ✅ Gestione utenti
│   ├── UserPermissions\            # ✅ Gestione permessi
│   └── Shared\                     # ✅ Layout e componenti
├── wwwroot\                        # File statici (CSS, JS, immagini)
└── [dependencies...]              # Tutte le DLL necessarie
```

---

## 👥 **UTENTI PREDEFINITI**

| 👤 Username | 🔐 Password | 🏷️ Ruolo | 📊 Accesso |
|-------------|-------------|-----------|-------------|
| `admin` | `123456` | **Administrator** | 🟢 **Completo** + impostazioni sistema |
| `senior` | `123456` | **UserSenior** | 🟡 **Avanzato** - dati riservati + reports |
| `user` | `123456` | **User** | 🔵 **Base** - solo dati generali |

---

## 🏗️ **ARCHITETTURA SISTEMA**

### 📊 **DATI RISERVATI** (Admin + Senior)
- ✅ **Banche**: Gestione conti bancari (17 view complete)
  - 📄 Lista, ➕ Crea, ✏️ Modifica, 🗑️ Elimina
- ✅ **Carte Credito**: Gestione carte di credito
- ✅ **Utenze**: Gestione utenze varie
- ✅ **Mail**: Gestione account email

### 📁 **DATI GENERALI** (Tutti gli utenti autorizzati)
- ✅ **Cancelleria**: Gestione fornitori (21 view complete)
  - 📄 Lista, ➕ Crea, ✏️ Modifica, 🗑️ Elimina
- ✅ **Utenti PC**: Gestione accessi computer
- ✅ **Altri Dati**: Dati generali vari
- ✅ **Entratel**: Accessi sistema fiscale
- ✅ **Utenti TS**: Accessi TeamSystem

### 👥 **GESTIONE SISTEMA** (Solo Administrator)
- ✅ **Utenti**: Creazione e gestione utenti
- ✅ **Permessi**: Configurazione autorizzazioni
- ✅ **Impostazioni**: Configurazione sistema

---

## 🔧 **REQUISITI TECNICI**

### 💻 **Server**
- Windows Server 2019/2022 o Windows 10/11
- IIS con modulo ASP.NET Core
- .NET 9.0 ASP.NET Core Runtime
- SQL Server 2019/2022 o Express

### 🗄️ **Database**
- Database: `ConsultingGroupDB`
- Utente: `consulting_user`
- Tutte le tabelle create automaticamente

### 🌐 **Web**
- Porta: 80 (HTTP) o 443 (HTTPS)
- Compatibile con tutti i browser moderni
- Design responsive per desktop/tablet

---

## 🆘 **SUPPORTO**

### 📚 **Documentazione**
- Guida completa: `INSTALLAZIONE_SERVER.md`
- Checklist: `CHECKLIST_INSTALLAZIONE.md`
- Comandi rapidi: `COMANDI_RAPIDI.md`

### 🔧 **Troubleshooting**
- Script verifica: `check_server.bat`
- Log applicazione: `C:\inetpub\logs\consulting\`
- Log IIS: `C:\inetpub\logs\LogFiles\`

### ⚡ **Comandi Rapidi**
```batch
# Verifica stato
check_server.bat

# Restart applicazione
%windir%\system32\inetsrv\appcmd.exe recycle apppool "ConsultingGroup"

# Test connessione
curl http://localhost
```

---

## ✅ **COSA È INCLUSO**

- ✅ **65 View complete** - Tutte le pagine del sistema
- ✅ **CRUD completi** - Create, Read, Update, Delete per ogni sezione
- ✅ **Sistema autorizzazioni** - 3 livelli di accesso
- ✅ **Sicurezza** - Autenticazione e controllo accessi
- ✅ **Design moderno** - UI responsive e professionale
- ✅ **Database automatico** - Creazione e seeding automatici
- ✅ **Installazione automatica** - Script tutto automatico
- ✅ **Documentazione completa** - Guide e procedure

---

## 🎉 **RISULTATO FINALE**

Dopo l'installazione avrai un sistema completo di **GESTIONE STUDIO** funzionante con:

- 🏢 **Dashboard** personalizzate per ogni ruolo
- 🔐 **Dati riservati** (banche, carte, utenze, mail)
- 📁 **Dati generali** condivisi (cancelleria, PC, Entratel, TS)
- 👥 **Gestione utenti** e permessi
- 📊 **Reports** e statistiche
- 🔒 **Sicurezza** enterprise-level

**URL Accesso**: `http://localhost`  
**Login Amministratore**: `admin` / `123456`

---

**🚀 PRONTO PER L'INSTALLAZIONE IN: C:\inetpub\consulting.web**




