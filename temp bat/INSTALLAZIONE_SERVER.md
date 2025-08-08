# 🚀 GUIDA INSTALLAZIONE SERVER - GESTIONE STUDIO CONSULTING GROUP SRL

## 📋 PREREQUISITI SERVER

### 1. SOFTWARE RICHIESTO
- **Windows Server 2019/2022** o **Windows 10/11**
- **IIS (Internet Information Services)** con moduli ASP.NET Core
- **.NET 9.0 Runtime** (ASP.NET Core Runtime)
- **SQL Server 2019/2022** o **SQL Server Express**

### 2. INSTALLAZIONE .NET 9.0 RUNTIME
1. Scaricare da: https://dotnet.microsoft.com/download/dotnet/9.0
2. Installare: **ASP.NET Core Runtime 9.0.x** (Hosting Bundle)
3. Riavviare il server dopo l'installazione

### 3. CONFIGURAZIONE IIS
1. Aprire **Gestione IIS**
2. Verificare che sia installato il modulo **AspNetCoreModuleV2**
3. Se mancante, installare il **ASP.NET Core Hosting Bundle**

---

## 📁 STRUTTURA CARTELLE SERVER

```
C:\inetpub\consulting.web\          # Cartella principale applicazione
├── wwwroot\                        # File statici (CSS, JS, immagini)
├── Views\                          # View Razor (tutte incluse)
├── appsettings.json               # Configurazione sviluppo
├── appsettings.Production.json    # Configurazione produzione
├── web.config                     # Configurazione IIS
├── ConsultingGroup.dll            # Assembly principale
└── [altri file .dll e dependencies]

C:\inetpub\logs\consulting\         # Cartella log (da creare)
C:\Database\Consulting\             # Cartella database (opzionale)
```

---

## 🗄️ CONFIGURAZIONE DATABASE

### 1. CREAZIONE DATABASE
```sql
-- Su SQL Server Management Studio
CREATE DATABASE ConsultingGroupDB
GO

-- Creare login applicazione
CREATE LOGIN [consulting_user] WITH PASSWORD = 'Password_Sicura_123!'
GO

USE ConsultingGroupDB
GO

-- Creare utente nel database
CREATE USER [consulting_user] FOR LOGIN [consulting_user]
GO

-- Assegnare permessi
ALTER ROLE [db_datareader] ADD MEMBER [consulting_user]
ALTER ROLE [db_datawriter] ADD MEMBER [consulting_user]
ALTER ROLE [db_ddladmin] ADD MEMBER [consulting_user]
GO
```

### 2. STRING DI CONNESSIONE
Configurare in `appsettings.Production.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=NOME_SERVER;Database=ConsultingGroupDB;User Id=consulting_user;Password=Password_Sicura_123!;TrustServerCertificate=true;MultipleActiveResultSets=true;"
  }
}
```

---

## 🔧 PROCEDURA DI INSTALLAZIONE

### STEP 1: PREPARAZIONE CARTELLE
1. Creare cartella destinazione:
   ```cmd
   mkdir "C:\inetpub\consulting.web"
   mkdir "C:\inetpub\logs\consulting"
   ```

2. Impostare permessi:
   - **IIS_IUSRS**: Lettura ed esecuzione
   - **NETWORK SERVICE**: Controllo completo su logs

### STEP 2: COPIA FILES
1. Copiare TUTTO il contenuto dalla cartella `publish` a `C:\inetpub\consulting.web`
2. Verificare che siano presenti:
   - ✅ ConsultingGroup.dll
   - ✅ web.config
   - ✅ wwwroot\ (completa)
   - ✅ Views\ (tutte le 65 view)
   - ✅ appsettings.Production.json

### STEP 3: CONFIGURAZIONE IIS
1. Aprire **Gestione IIS**
2. Espandere il server locale
3. Click destro su **Siti** → **Aggiungi sito Web**
4. Configurare:
   - **Nome sito**: `Consulting Group - Gestione Studio`
   - **Percorso fisico**: `C:\inetpub\consulting.web`
   - **Porta**: `80` (o `443` per HTTPS)
   - **Nome host**: `consulting.local` (opzionale)

### STEP 4: CONFIGURAZIONE POOL APPLICAZIONI
1. Andare in **Pool di applicazioni**
2. Selezionare il pool del sito (solitamente ha lo stesso nome)
3. **Impostazioni avanzate**:
   - **Versione .NET CLR**: `Nessun codice gestito`
   - **Modalità pipeline gestita**: `Integrata`
   - **Identità**: `ApplicationPoolIdentity`
   - **Avvia automaticamente**: `True`

### STEP 5: CONFIGURAZIONE AMBIENTE
1. Nel pool applicazioni → **Variabili di ambiente**
2. Aggiungere:
   - **Nome**: `ASPNETCORE_ENVIRONMENT`
   - **Valore**: `Production`

### STEP 6: ESECUZIONE MIGRATION DATABASE
1. Aprire **Prompt dei comandi** come amministratore
2. Navigare in: `C:\inetpub\consulting.web`
3. Eseguire:
   ```cmd
   dotnet ConsultingGroup.dll --migrate
   ```
   OPPURE eseguire script SQL manualmente

---

## 🔐 CONFIGURAZIONE SICUREZZA

### 1. HTTPS (RACCOMANDATO)
1. Ottenere certificato SSL
2. In IIS → **Associazioni sito** → Aggiungere HTTPS porta 443
3. Configurare redirect HTTP → HTTPS

### 2. FIREWALL
- Aprire porta **80** (HTTP)
- Aprire porta **443** (HTTPS)
- Aprire porta **1433** (SQL Server) solo se necessario

### 3. BACKUP
- Database: Backup automatico giornaliero
- File applicazione: Backup settimanale cartella `C:\inetpub\consulting.web`

---

## 👥 UTENTI PREDEFINITI

Il sistema include utenti di test:

| Username | Password | Ruolo | Descrizione |
|----------|----------|--------|-------------|
| `admin` | `123456` | Administrator | Accesso completo + impostazioni sistema |
| `senior` | `123456` | UserSenior | Accesso dati riservati + reports |
| `user` | `123456` | User | Accesso base dati generali |

**⚠️ IMPORTANTE**: Cambiare le password predefinite prima della produzione!

---

## 🧪 TEST POST-INSTALLAZIONE

### 1. VERIFICA SITO
1. Aprire browser: `http://localhost` o `http://consulting.local`
2. Verificare che appaia la pagina di login
3. Testare login con `admin/123456`

### 2. VERIFICA FUNZIONALITÀ
1. **Accesso**: ✅ Login/Logout
2. **Dati Riservati**: ✅ CRUD completo (Banche, Carte, Utenze, Mail)
3. **Dati Generali**: ✅ CRUD completo (Cancelleria, UtentiPC, AltriDati, Entratel, UtentiTS)
4. **Gestione Utenti**: ✅ (solo Administrator)
5. **Permessi**: ✅ Verifica accessi per ruolo

### 3. VERIFICA LOG
- Controllare log in: `C:\inetpub\logs\consulting`
- Verificare log IIS in: `C:\inetpub\logs\LogFiles`

---

## 🔧 RISOLUZIONE PROBLEMI

### Errore 500.30 - ASP.NET Core app failed to start
- Verificare installazione .NET 9.0 Runtime
- Controllare permessi cartella
- Verificare web.config

### Errore connessione database
- Verificare string di connessione
- Testare connessione SQL Server
- Verificare permessi utente database

### Pagine view non trovate
- Verificare che cartella `Views` sia stata copiata
- Controllare permessi lettura IIS_IUSRS

---

## 📞 SUPPORTO

Per problemi tecnici:
1. Controllare log applicazione
2. Verificare Event Viewer Windows
3. Consultare documentazione ASP.NET Core

---

**✅ Installazione completata con successo!**
**🎉 Sistema GESTIONE STUDIO - CONSULTING GROUP SRL operativo!**
