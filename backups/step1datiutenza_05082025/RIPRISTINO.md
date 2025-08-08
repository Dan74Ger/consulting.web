# ISTRUZIONI RIPRISTINO - STEP1DATIUTENZA

📅 **Data Backup:** 05 Agosto 2025  
🔖 **Versione:** step1datiutenza  
📝 **Descrizione:** Completamento sezione Dati Generali con UtentiTS

## 🎯 PREREQUISITI

### Software Richiesto
- ✅ Visual Studio 2022 / VS Code con C# extension
- ✅ .NET 8.0 SDK
- ✅ SQL Server Express / SQL Server LocalDB
- ✅ SQL Server Management Studio (consigliato)

### Competenze Richieste
- Conoscenza base ASP.NET Core MVC
- Familiarità con Entity Framework Core
- Capacità gestione database SQL Server

## 🔧 PROCEDURA RIPRISTINO COMPLETA

### STEP 1: Preparazione Ambiente

```bash
# Verificare .NET 8.0 installato
dotnet --version

# Creare cartella progetto ripristinato
mkdir C:\dev\prova-restored
cd C:\dev\prova-restored
```

### STEP 2: Ripristino Database

**Opzione A - Ripristino da Backup (RACCOMANDATO):**

```sql
-- In SQL Server Management Studio o Azure Data Studio
-- 1. Connettersi a IT15\SQLEXPRESS
-- 2. Tasto destro su Databases > Restore Database
-- 3. Selezionare "Device" e scegliere il file:
--    C:\dev\prova\backups\step1datiutenza_05082025\Consulting_step1datiutenza_05082025.bak

-- Oppure via script:
RESTORE DATABASE [Consulting_Restored] 
FROM DISK = N'C:\dev\prova\backups\step1datiutenza_05082025\Consulting_step1datiutenza_05082025.bak'
WITH REPLACE, 
MOVE 'Consulting' TO 'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Consulting_Restored.mdf',
MOVE 'Consulting_Log' TO 'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLEXPRESS\MSSQL\DATA\Consulting_Restored.ldf'
```

**Opzione B - Database da Zero:**
```bash
# Solo se non funziona il ripristino da backup
dotnet ef database drop --force
dotnet ef database update
```

### STEP 3: Ripristino Codice Sorgente

```bash
# Copiare tutti i file da backup
xcopy "C:\dev\prova\backups\step1datiutenza_05082025\sourcecode" "C:\dev\prova-restored" /E /I

# Ripristinare pacchetti NuGet
dotnet restore

# Pulire cache se necessario
dotnet clean
```

### STEP 4: Configurazione Connection String

**Modificare `appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=IT15\\SQLEXPRESS;Database=Consulting_Restored;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Se database originale:**
```json
"DefaultConnection": "Server=IT15\\SQLEXPRESS;Database=Consulting;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true"
```

### STEP 5: Compilazione e Test

```bash
# Compilare progetto
dotnet build

# Se successo, avviare applicazione
dotnet run

# L'applicazione sarà disponibile su:
# http://localhost:8080 (o porta indicata nei log)
```

## 🧪 TESTING COMPLETO

### Test Login e Autorizzazioni

**Credenziali di test:**
```bash
admin/123456    # Administrator - Accesso completo
senior/123456   # UserSenior - Accesso reports  
user/123456     # User - Accesso limitato
```

**Verifiche da eseguire:**
- [ ] Login con ogni ruolo
- [ ] Logout corretto
- [ ] Redirect autorizzazioni appropriati

### Test Dati Generali (5 Sezioni)

**Per OGNI ruolo utente verificare:**

1. **📊 Dashboard Statistiche**
   - [ ] Card con contatori delle 5 sezioni
   - [ ] Statistiche aggiornate real-time
   - [ ] Layout responsive

2. **📝 Cancelleria (Info Blue)**
   - [ ] Lista completa record
   - [ ] Create: nuovo fornitore
   - [ ] Edit: modifica esistente
   - [ ] Delete: eliminazione con conferma

3. **💻 Utenti PC (Primary Blue)**
   - [ ] Lista completa record
   - [ ] Create: nuove credenziali PC
   - [ ] Edit: modifica esistente
   - [ ] Delete: eliminazione con conferma

4. **📊 Altri Dati (Success Green)**
   - [ ] Lista completa record
   - [ ] Create: nuovi dati generici
   - [ ] Edit: modifica esistente
   - [ ] Delete: eliminazione con conferma

5. **🏛️ ENTRATEL (Secondary Gray)**
   - [ ] Lista completa record
   - [ ] Create: nuovi accessi fiscali
   - [ ] Edit: modifica esistente
   - [ ] Delete: eliminazione con conferma

6. **🛡️ Utenti TS (Danger Red) ⭐ NUOVO!**
   - [ ] Lista completa record
   - [ ] Create: nuove credenziali TS
   - [ ] Edit: modifica esistente
   - [ ] Delete: eliminazione con conferma

### Test UI/UX

**Verifiche interfaccia:**
- [ ] Design responsive mobile/desktop
- [ ] Navigazione breadcrumb
- [ ] Messaggi successo/errore
- [ ] Azioni rapide funzionanti
- [ ] Tabelle responsive
- [ ] Link "Visualizza Tutti" per ogni sezione

## 🔍 TROUBLESHOOTING

### Errore Database Connection

**Problema:** Cannot connect to SQL Server  
**Soluzioni:**
```bash
# Verificare servizio SQL Server Express
services.msc → SQL Server (SQLEXPRESS) → Start

# Verificare instanza
sqlcmd -S "IT15\SQLEXPRESS" -Q "SELECT @@VERSION"

# Verificare connection string in appsettings.json
```

### Errore Migration/Schema

**Problema:** Schema database non allineato  
**Soluzioni:**
```bash
# Verificare migration applicate
dotnet ef migrations list

# Reset completo database
dotnet ef database drop --force
dotnet ef database update

# Oppure ripristino da backup
```

### Errore Compilation

**Problema:** Build errors o classi mancanti  
**Soluzioni:**
```bash
# Pulire cache
dotnet clean
dotnet nuget locals all --clear

# Restore packages
dotnet restore --force

# Verificare file copiati correttamente
```

### Errore UtentiTS Missing

**Problema:** Tabella UtentiTS non trovata  
**Verifiche:**
```sql
-- Controllare se tabella esiste
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'UtentiTS'

-- Se mancante, applicare migration specifica
dotnet ef migrations add RestoreUtentiTS
dotnet ef database update
```

### Errore Permission Denied

**Problema:** Utenti non possono accedere sezioni  
**Verifiche:**
```bash
# Controllare UserPermissionsService
# Verificare seed data utenti esistenti
# Controllare UserPermissionAttribute sui controller
```

## 📁 STRUTTURA FILES RIPRISTINATI

```
C:\dev\prova-restored/
├── Controllers/
│   ├── DatiUtenzaExtraController.cs    # Controller Dati Generali
│   ├── AccountController.cs            # Autenticazione
│   └── ...                            # Altri controller
├── Models/
│   ├── UtentiTS.cs                     # ⭐ Nuovo modello
│   ├── Cancelleria.cs                 # Altri modelli
│   └── ...
├── Views/
│   ├── DatiUtenzaExtra/               # ⭐ Views Dati Generali
│   │   ├── UtentiTS.cshtml            # Lista UtentiTS
│   │   ├── CreateUtentiTS.cshtml      # Form creazione
│   │   ├── EditUtentiTS.cshtml        # Form modifica
│   │   ├── DeleteUtentiTS.cshtml      # Conferma eliminazione
│   │   └── Index.cshtml               # Dashboard aggiornata
│   └── ...
├── ViewModels/
│   ├── DatiUtenzaExtraViewModel.cs    # ⭐ Include UtentiTS
│   └── ...
├── Data/
│   ├── ApplicationDbContext.cs        # ⭐ Include DbSet<UtentiTS>
│   └── Migrations/
│       └── AddUtentiTSTable.cs        # ⭐ Migration UtentiTS
├── Services/
│   └── UserPermissionsService.cs      # Gestione permessi
├── appsettings.json                   # Configurazione
├── Program.cs                         # Entry point
└── README.md                          # Documentazione aggiornata
```

## 🎯 VALIDAZIONE FUNZIONAMENTO

### Checklist Finale

**✅ Database:**
- [ ] Connessione SQL Server funzionante
- [ ] Tabelle presenti (inclusa UtentiTS)
- [ ] Utenti test esistenti con ruoli corretti

**✅ Applicazione:**
- [ ] Compilation senza errori
- [ ] Avvio senza eccezioni
- [ ] Login/logout funzionante

**✅ Funzionalità UtentiTS:**
- [ ] Dashboard mostra contatore UtentiTS
- [ ] Menu include link UtentiTS
- [ ] CRUD completo funzionante
- [ ] Azioni rapide operative

**✅ UI/UX:**
- [ ] Design responsive
- [ ] Navigazione intuitiva
- [ ] Messaggi feedback utente

## 🆘 SUPPORTO

**In caso di problemi persistenti:**

1. **Controllare log applicazione** in console durante `dotnet run`
2. **Verificare Event Viewer** per errori SQL Server
3. **Testare database** con SQL Server Management Studio
4. **Confrontare** con progetto originale funzionante

**File di riferimento:**
- `README_BACKUP.md` per stato completo progetto
- `backup_database.sql` per script SQL
- Progetto originale in `C:\dev\prova` (se ancora disponibile)

---

## 📝 NOTES IMPORTANTI

⚠️ **Attenzione:**
- Questo backup include la **nuova sezione UtentiTS completa**
- Il database contiene **migration AddUtentiTSTable applicata**
- Sistema autorizzazioni **funziona per tutti i ruoli**
- Tutte le **5 sezioni Dati Generali sono operative**

✅ **Garanzie:**
- **CRUD completo** su tutte le sezioni
- **Dashboard con statistiche** aggiornate
- **Design responsive** e user-friendly
- **Sistema sicurezza** robusto

🎯 **Obiettivo:**
Il sistema ripristinato deve essere **identico** a quello originale con tutte le funzionalità UtentiTS operative per tutti i ruoli utente.

---
**📧 Documento creato automaticamente - Sistema step1datiutenza**  
**🗓️ Data: 05 Agosto 2025**  
**✅ Status: PRODUCTION READY**