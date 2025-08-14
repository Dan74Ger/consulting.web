# 🔄 RIPRISTINO COMPLETO - GESTIONE STUDIO

## 📋 PANORAMICA RIPRISTINO

Questo documento fornisce le istruzioni complete per ripristinare il sistema **GESTIONE STUDIO - CONSULTING GROUP** a partire dai backup disponibili. Il sistema include database SQL Server, codice sorgente completo e tutte le configurazioni necessarie.

## 📦 BACKUP DISPONIBILI (AGGIORNATI)

### 🗄️ **DATABASE SQL SERVER**
- **File Recente:** `Consulting_FINALE_20250804_180542.bak`
- **Dimensione:** 6.3 MB
- **Contenuto:** Database completo con 17 tabelle
- **Percorso:** `C:\dev\prova\backups\`

### 💾 **CODICE SORGENTE COMPLETO**
- **File Recente:** `ConsultingGroup_FINALE_20250804_180542.zip`
- **Dimensione:** 11.4 MB 
- **Contenuto:** 223 file di progetto completi
- **Percorso:** `C:\dev\prova\backups\`

### 📊 **CONTENUTO BACKUP DATABASE**
```
✅ Tabelle Identity (8): AspNetUsers, AspNetRoles, AspNetUserRoles, etc.
✅ Tabelle Dati Utenza (9): Banche, CarteCredito, Utenze, Mail, Cancelleria, UtentiPC, AltriDati, Entratel, UserPermissions
✅ Tabelle Sistema (1): __EFMigrationsHistory
✅ Dati Utenti Test: admin, senior, user (password: 123456)
✅ Configurazioni Permessi: Sistema a 3 livelli completo
```

---

## 🔧 PREREQUISITI SISTEMA

### 📋 **Software Richiesto**
- **Windows 10/11** o **Windows Server 2019+**
- **SQL Server Express 2019+** installato e funzionante
- **.NET 8.0 SDK** o superior
- **IIS** (opzionale, per hosting produzione)
- **Visual Studio 2022** o **VS Code** (per sviluppo)

### 🔍 **Verifica Prerequisiti**
```powershell
# Verifica .NET SDK
dotnet --version
# Output atteso: 8.0.x o superiore

# Verifica SQL Server
sqlcmd -S "IT15\SQLEXPRESS" -Q "SELECT @@VERSION"
# Deve connettersi senza errori

# Verifica PowerShell
$PSVersionTable.PSVersion
# Output atteso: 5.1+ o 7.0+
```

---

## 📂 METODO 1: RIPRISTINO MANUALE

### 1️⃣ **RIPRISTINO DATABASE**

#### **Opzione A: SQL Server Management Studio (SSMS)**
```sql
-- 1. Aprire SSMS e connettersi a IT15\SQLEXPRESS
-- 2. Click destro su "Databases" → "Restore Database"
-- 3. Selezionare "Device" → "Add"
-- 4. Scegliere: C:\dev\prova\backups\Consulting_FINALE_20250804_180542.bak
-- 5. Verificare che "Consulting" sia il database di destinazione
-- 6. In "Options": spuntare "Overwrite the existing database"
-- 7. Click "OK" per avviare il ripristino
```

#### **Opzione B: Command Line (sqlcmd)**
```cmd
sqlcmd -S "IT15\SQLEXPRESS" -Q "RESTORE DATABASE [Consulting] FROM DISK = 'C:\dev\prova\backups\Consulting_FINALE_20250804_180542.bak' WITH REPLACE"
```

#### **Opzione C: PowerShell**
```powershell
$backupFile = "C:\dev\prova\backups\Consulting_FINALE_20250804_180542.bak"
$sqlCommand = "RESTORE DATABASE [Consulting] FROM DISK = '$backupFile' WITH REPLACE"
sqlcmd -S "IT15\SQLEXPRESS" -Q $sqlCommand
```

### 2️⃣ **RIPRISTINO CODICE SORGENTE**

#### **Estrazione Archive ZIP**
```powershell
# Creare cartella destinazione
New-Item -ItemType Directory -Path "C:\dev\prova_restored" -Force

# Estrarre backup codice
$zipFile = "C:\dev\prova\backups\ConsultingGroup_FINALE_20250804_180542.zip"
Expand-Archive -Path $zipFile -DestinationPath "C:\dev\prova_restored" -Force

# Navigare nella cartella
cd C:\dev\prova_restored
```

### 3️⃣ **CONFIGURAZIONE PROGETTO**

#### **Ripristino Pacchetti NuGet**
```bash
# Navigare nella cartella del progetto
cd C:\dev\prova_restored

# Ripristinare dipendenze
dotnet restore

# Verificare che non ci siano errori
dotnet build
```

#### **Verifica Connection String**
```json
// Verificare in appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=IT15\\SQLEXPRESS;Database=Consulting;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### 4️⃣ **AVVIO APPLICAZIONE**
```bash
# Avvio per test
dotnet run --urls "http://localhost:5000"

# Test login
# URL: http://localhost:5000
# Utenti test: admin/123456, senior/123456, user/123456
```

---

## 🚀 METODO 2: RIPRISTINO AUTOMATIZZATO

### **Script PowerShell Completo**

Creare un file `ripristino_automatico.ps1`:

```powershell
param(
    [string]$BackupPath = "C:\dev\prova\backups",
    [string]$RestorePath = "C:\dev\prova_restored",
    [string]$SqlServer = "IT15\SQLEXPRESS",
    [string]$DatabaseName = "Consulting"
)

Write-Host "=== RIPRISTINO AUTOMATICO GESTIONE STUDIO ===" -ForegroundColor Green
Write-Host "Backup Path: $BackupPath" -ForegroundColor Yellow
Write-Host "Restore Path: $RestorePath" -ForegroundColor Yellow

# 1. Trova i file di backup più recenti
$dbBackup = Get-ChildItem "$BackupPath\Consulting_FINALE_*.bak" | Sort-Object LastWriteTime -Descending | Select-Object -First 1
$sourceBackup = Get-ChildItem "$BackupPath\ConsultingGroup_FINALE_*.zip" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if (-not $dbBackup) {
    Write-Error "Backup database non trovato in $BackupPath"
    exit 1
}

if (-not $sourceBackup) {
    Write-Error "Backup codice sorgente non trovato in $BackupPath"
    exit 1
}

Write-Host "Database Backup: $($dbBackup.Name)" -ForegroundColor Cyan
Write-Host "Source Backup: $($sourceBackup.Name)" -ForegroundColor Cyan

# 2. Backup database esistente (se presente)
Write-Host "Backup database esistente..." -ForegroundColor Yellow
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
try {
    sqlcmd -S $SqlServer -Q "BACKUP DATABASE [$DatabaseName] TO DISK = '$BackupPath\${DatabaseName}_PreRestore_$timestamp.bak'" 2>$null
    Write-Host "✅ Backup preesistente creato" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Database non esistente, continuando..." -ForegroundColor Yellow
}

# 3. Ripristino database
Write-Host "Ripristino database da $($dbBackup.Name)..." -ForegroundColor Yellow
try {
    $restoreCmd = "RESTORE DATABASE [$DatabaseName] FROM DISK = '$($dbBackup.FullName)' WITH REPLACE"
    sqlcmd -S $SqlServer -Q $restoreCmd
    Write-Host "✅ Database ripristinato con successo" -ForegroundColor Green
} catch {
    Write-Error "❌ Errore durante ripristino database: $_"
    exit 1
}

# 4. Creazione cartella destinazione
Write-Host "Creazione cartella destinazione..." -ForegroundColor Yellow
if (Test-Path $RestorePath) {
    Remove-Item $RestorePath -Recurse -Force
}
New-Item -ItemType Directory -Path $RestorePath -Force | Out-Null

# 5. Estrazione codice sorgente
Write-Host "Estrazione codice sorgente..." -ForegroundColor Yellow
try {
    Expand-Archive -Path $sourceBackup.FullName -DestinationPath $RestorePath -Force
    Write-Host "✅ Codice sorgente estratto con successo" -ForegroundColor Green
} catch {
    Write-Error "❌ Errore durante estrazione: $_"
    exit 1
}

# 6. Ripristino pacchetti NuGet
Write-Host "Ripristino pacchetti NuGet..." -ForegroundColor Yellow
Push-Location $RestorePath
try {
    dotnet restore
    Write-Host "✅ Pacchetti NuGet ripristinati" -ForegroundColor Green
} catch {
    Write-Error "❌ Errore durante ripristino pacchetti: $_"
    Pop-Location
    exit 1
}

# 7. Build progetto
Write-Host "Build progetto..." -ForegroundColor Yellow
try {
    dotnet build --configuration Release
    Write-Host "✅ Progetto compilato con successo" -ForegroundColor Green
} catch {
    Write-Error "❌ Errore durante build: $_"
    Pop-Location
    exit 1
}

Pop-Location

# 8. Verifica finale
Write-Host "Verifica configurazione database..." -ForegroundColor Yellow
try {
    $testQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'"
    $tableCount = sqlcmd -S $SqlServer -d $DatabaseName -Q $testQuery -h -1 | Where-Object {$_.Trim() -match '^\d+$'}
    Write-Host "✅ Database verificato: $tableCount tabelle trovate" -ForegroundColor Green
} catch {
    Write-Warning "⚠️ Impossibile verificare database: $_"
}

Write-Host ""
Write-Host "=== RIPRISTINO COMPLETATO ===" -ForegroundColor Green
Write-Host "📂 Codice ripristinato in: $RestorePath" -ForegroundColor White
Write-Host "🗄️ Database ripristinato: $DatabaseName su $SqlServer" -ForegroundColor White
Write-Host "🚀 Per avviare: cd '$RestorePath' && dotnet run --urls 'http://localhost:5000'" -ForegroundColor White
Write-Host "🔐 Login test: admin/123456, senior/123456, user/123456" -ForegroundColor White
```

### **Esecuzione Script Automatico**
```powershell
# Eseguire da PowerShell come Amministratore
.\ripristino_automatico.ps1

# Con parametri personalizzati
.\ripristino_automatico.ps1 -BackupPath "D:\Backups" -RestorePath "D:\Projects\Restored"
```

---

## ✅ VERIFICA POST-RIPRISTINO

### 🔍 **Test Database**
```sql
-- Verifica tabelle principali
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;

-- Verifica utenti test
SELECT UserName, Email FROM AspNetUsers;

-- Verifica dati di esempio
SELECT COUNT(*) as RecordCount, 'Banche' as Tabella FROM Banche
UNION ALL
SELECT COUNT(*), 'CarteCredito' FROM CarteCredito
UNION ALL  
SELECT COUNT(*), 'UserPermissions' FROM UserPermissions;
```

### 🌐 **Test Applicazione**
```bash
# Avvio applicazione
cd C:\dev\prova_restored
dotnet run --urls "http://localhost:5000"

# Test browser
# URL: http://localhost:5000
# Login: admin / 123456
```

### ✅ **Checklist Verifica**
- [ ] Database Consulting ripristinato correttamente
- [ ] 17 tabelle presenti nel database
- [ ] Codice sorgente estratto (223 file)
- [ ] Progetto compila senza errori (`dotnet build`)
- [ ] Applicazione si avvia (`dotnet run`)
- [ ] Login admin/123456 funziona
- [ ] Dashboard Administrator accessibile
- [ ] Sezioni Dati Utenza operative
- [ ] Menu dinamico basato su permessi funziona

---

## 🚨 RISOLUZIONE PROBLEMI

### ❌ **Errore: Database già esistente**
```sql
-- Eliminare database esistente
USE master;
DROP DATABASE IF EXISTS [Consulting];
-- Poi ripetere il ripristino
```

### ❌ **Errore: Permission denied**
```powershell
# Eseguire PowerShell come Amministratore
# Verificare permessi SQL Server
# Verificare che l'utente Windows abbia accesso a SQL Server
```

### ❌ **Errore: .NET SDK non trovato**
```bash
# Scaricare e installare .NET 8.0 SDK
# https://dotnet.microsoft.com/download/dotnet/8.0
```

### ❌ **Errore: Connection String non valida**
```json
// Verificare e correggere in appsettings.json
"DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=Consulting;Trusted_Connection=true;TrustServerCertificate=true"
```

### ❌ **Errore: Port 5000 occupata**
```bash
# Usare porta alternativa
dotnet run --urls "http://localhost:5001"
```

---

## 📊 INFORMAZIONI BACKUP

### 📅 **Backup Recenti**
- **Data Creazione:** 04 Agosto 2025, 18:05
- **Versione Progetto:** 1.0 - Sistema Completo
- **Stato:** ✅ Produzione Ready
- **Framework:** ASP.NET Core 8.0 MVC

### 🔢 **Statistiche Database Ripristinato**
```
👥 Utenti Test: 3 (admin, senior, user)
🔐 Ruoli: 3 (Administrator, UserSenior, User)  
📋 Tabelle Dati: 9 (Banche, Carte, etc.)
⚙️ Tabelle Sistema: 8 (AspNet*, Migrations)
🎯 Permessi: 3 controlli principali
💾 Dimensione DB: ~6.3 MB
```

### 🏗️ **Architettura Ripristinata**
- **Autenticazione:** ASP.NET Core Identity
- **Autorizzazione:** Sistema ibrido ruoli + permessi granulari
- **Database:** Entity Framework Core con SQL Server
- **Frontend:** Bootstrap 5 + Font Awesome
- **Hosting:** IIS Ready (genera DLL)

---

## 📞 SUPPORTO

### 🏢 **Informazioni Progetto**
- **Nome:** GESTIONE STUDIO - CONSULTING GROUP
- **Versione:** 1.0 - Sistema Completo
- **Framework:** ASP.NET Core 8.0 MVC
- **Database:** SQL Server Express

### 🔗 **URLs Importanti Post-Ripristino**
```
🏠 Homepage:           http://localhost:5000
🔐 Login:              http://localhost:5000/Account/Login
👑 Admin Dashboard:    http://localhost:5000/Admin
👔 Senior Dashboard:   http://localhost:5000/Senior  
👤 User Dashboard:     http://localhost:5000/User
🏦 Dati Riservati:     http://localhost:5000/DatiUtenza
📋 Dati Generali:     http://localhost:5000/DatiUtenzaExtra
```

### 📋 **Credenziali Test Post-Ripristino**
```
Administrator: admin    / 123456
UserSenior:    senior   / 123456  
User:          user     / 123456
```

---

*🎯 Procedura di ripristino completa per sistema di gestione studio professionale. Per assistenza aggiuntiva, consultare il README.md principale.*