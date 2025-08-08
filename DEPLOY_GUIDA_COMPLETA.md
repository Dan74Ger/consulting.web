# 🚀 **GUIDA COMPLETA DEPLOY - CONSULTING GROUP**

Questa guida ti permette di rifare il deployment completo dal tuo PC di sviluppo al server con la configurazione esatta che funziona.

---

## 📋 **CONFIGURAZIONE SERVER FUNZIONANTE**

- **Server IP**: 192.168.1.112
- **SQL Server**: srv-dc\SQLEXPRESS
- **Database**: Consulting
- **Autenticazione**: Windows Authentication
- **URL Sito**: http://192.168.1.112
- **Percorso IIS**: C:\inetpub\consulting.web
- **Application Pool**: consulting.web
- **Sito IIS**: consulting.web

---

## 🔧 **PREREQUISITI SUL SERVER (GIÀ CONFIGURATI)**

✅ IIS installato e funzionante  
✅ .NET 9.0 Runtime/Hosting Bundle installato  
✅ SQL Server Express (srv-dc\SQLEXPRESS) funzionante  
✅ Database "Consulting" creato  
✅ Application Pool "consulting.web" configurato  
✅ Sito IIS "consulting.web" configurato  
✅ Permessi database per IIS APPPOOL\consulting.web configurati  

---

## 📦 **STEP 1: PREPARAZIONE SUL PC DI SVILUPPO**

### **1.1 Navigare nella cartella progetto**
```powershell
cd C:\dev\prova
```

### **1.2 Verificare che tutto sia aggiornato**
```powershell
# Verifica che il progetto compili
dotnet build --configuration Release
```

### **1.3 Creare il publish aggiornato**
```powershell
# Elimina publish precedente
if (Test-Path "publish") { Remove-Item -Recurse -Force "publish" }

# Crea nuovo publish
dotnet publish --configuration Release --output publish
```

### **1.4 Verificare che i file siano presenti**
```powershell
# Controlla file principali
Test-Path "publish\ConsultingGroup.dll"
Test-Path "publish\appsettings.json"
Get-ChildItem "publish\Views" -Recurse | Measure-Object | Select-Object Count
```

---

## 📁 **STEP 2: PREPARAZIONE FILE CONFIGURAZIONE**

### **2.1 Creare appsettings.Server.json**
```powershell
@'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=srv-dc\\SQLEXPRESS;Database=Consulting;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true;Connection Timeout=30;Command Timeout=60;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Identity": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  },
  "AllowedHosts": "*",
  "Application": {
    "BaseUrl": "http://192.168.1.112"
  }
}
'@ | Out-File -FilePath "appsettings.Server.json" -Encoding UTF8
```

### **2.2 Creare web.config corretto**
```powershell
@'
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <location path="." inheritInChildApplications="false">
    <system.webServer>
      <handlers>
        <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
      </handlers>
      <aspNetCore processPath="dotnet" arguments=".\ConsultingGroup.dll" stdoutLogEnabled="true" stdoutLogFile=".\logs\stdout" hostingModel="InProcess">
        <environmentVariables>
          <environmentVariable name="ASPNETCORE_ENVIRONMENT" value="Production" />
          <environmentVariable name="ASPNETCORE_URLS" value="http://0.0.0.0:80" />
          <environmentVariable name="DOTNET_PRINT_TELEMETRY_MESSAGE" value="false" />
        </environmentVariables>
      </aspNetCore>
    </system.webServer>
  </location>
</configuration>
'@ | Out-File -FilePath "web.config.server" -Encoding UTF8
```

---

## 🔄 **STEP 3: SCRIPT DEPLOY AUTOMATICO**

### **3.1 Creare DEPLOY_TO_SERVER.ps1**
```powershell
@'
# ============================================
# DEPLOY TO SERVER - CONSULTING GROUP
# ============================================

Write-Host "============================================" -ForegroundColor Cyan
Write-Host "DEPLOY TO SERVER - CONSULTING GROUP" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan

$serverPath = "\\192.168.1.112\c$\inetpub\consulting.web"
$backupPath = "\\192.168.1.112\c$\inetpub\consulting.web.backup." + (Get-Date -Format "yyyyMMdd_HHmmss")

try {
    # [1] Test connessione server
    Write-Host "[1/8] Test connessione server..." -ForegroundColor Yellow
    if (-not (Test-Path "\\192.168.1.112\c$")) {
        throw "Server non raggiungibile. Verificare credenziali e connessione."
    }
    Write-Host "✅ Server raggiungibile" -ForegroundColor Green

    # [2] Backup esistente
    Write-Host "[2/8] Backup installazione esistente..." -ForegroundColor Yellow
    if (Test-Path $serverPath) {
        Copy-Item -Path $serverPath -Destination $backupPath -Recurse -Force
        Write-Host "✅ Backup creato: $backupPath" -ForegroundColor Green
    } else {
        Write-Host "ℹ️ Prima installazione" -ForegroundColor Cyan
    }

    # [3] Ferma Application Pool
    Write-Host "[3/8] Ferma Application Pool..." -ForegroundColor Yellow
    Invoke-Command -ComputerName "192.168.1.112" -ScriptBlock {
        Import-Module WebAdministration
        Stop-WebAppPool -Name "consulting.web" -ErrorAction SilentlyContinue
    }
    Write-Host "✅ Application Pool fermato" -ForegroundColor Green

    # [4] Crea cartelle
    Write-Host "[4/8] Preparazione cartelle..." -ForegroundColor Yellow
    if (-not (Test-Path $serverPath)) {
        New-Item -ItemType Directory -Path $serverPath -Force | Out-Null
    }
    Write-Host "✅ Cartelle pronte" -ForegroundColor Green

    # [5] Copia file applicazione
    Write-Host "[5/8] Copia file applicazione..." -ForegroundColor Yellow
    Copy-Item -Path "publish\*" -Destination $serverPath -Recurse -Force
    Write-Host "✅ File applicazione copiati" -ForegroundColor Green

    # [6] Copia configurazione server
    Write-Host "[6/8] Configurazione server..." -ForegroundColor Yellow
    Copy-Item -Path "appsettings.Server.json" -Destination "$serverPath\appsettings.Production.json" -Force
    Copy-Item -Path "web.config.server" -Destination "$serverPath\web.config" -Force
    Write-Host "✅ Configurazione applicata" -ForegroundColor Green

    # [7] Crea cartella DataProtection
    Write-Host "[7/8] Configurazione DataProtection..." -ForegroundColor Yellow
    $dataProtectionPath = "$serverPath\App_Data\DataProtection-Keys"
    if (-not (Test-Path $dataProtectionPath)) {
        New-Item -ItemType Directory -Path $dataProtectionPath -Force | Out-Null
    }
    Write-Host "✅ DataProtection configurato" -ForegroundColor Green

    # [8] Riavvia servizi
    Write-Host "[8/8] Riavvio servizi..." -ForegroundColor Yellow
    Invoke-Command -ComputerName "192.168.1.112" -ScriptBlock {
        Import-Module WebAdministration
        Start-WebAppPool -Name "consulting.web" -ErrorAction SilentlyContinue
        Start-IISSite -Name "consulting.web" -ErrorAction SilentlyContinue
    }
    Start-Sleep -Seconds 10
    Write-Host "✅ Servizi riavviati" -ForegroundColor Green

    # Test finale
    Write-Host ""
    Write-Host "============================================" -ForegroundColor Green
    Write-Host "🎉 DEPLOY COMPLETATO CON SUCCESSO!" -ForegroundColor Green
    Write-Host "============================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "📍 URL: http://192.168.1.112" -ForegroundColor Cyan
    Write-Host "🔐 Login: admin / 123456" -ForegroundColor Cyan
    Write-Host "📁 Percorso: C:\inetpub\consulting.web" -ForegroundColor White
    Write-Host "💾 Backup: $backupPath" -ForegroundColor White
    Write-Host ""

} catch {
    Write-Host ""
    Write-Host "❌ ERRORE DURANTE IL DEPLOY!" -ForegroundColor Red
    Write-Host "Errore: $($_.Exception.Message)" -ForegroundColor Red
    
    if (Test-Path $backupPath) {
        Write-Host ""
        Write-Host "💾 Ripristino backup..." -ForegroundColor Yellow
        Remove-Item -Path $serverPath -Recurse -Force -ErrorAction SilentlyContinue
        Move-Item -Path $backupPath -Destination $serverPath
        Write-Host "✅ Backup ripristinato" -ForegroundColor Green
    }
    
    exit 1
}
'@ | Out-File -FilePath "DEPLOY_TO_SERVER.ps1" -Encoding UTF8
```

---

## 🚀 **STEP 4: ESECUZIONE DEPLOY**

### **4.1 Eseguire il deploy**
```powershell
# Eseguire come Amministratore
.\DEPLOY_TO_SERVER.ps1
```

### **4.2 Se il deploy remoto non funziona, usa il metodo manuale:**

#### **A. Copia file sul server:**
```powershell
# Copia manuale se il deploy automatico fallisce
Copy-Item -Path "publish\*" -Destination "\\192.168.1.112\c$\inetpub\consulting.web\" -Recurse -Force
Copy-Item -Path "appsettings.Server.json" -Destination "\\192.168.1.112\c$\inetpub\consulting.web\appsettings.Production.json" -Force
Copy-Item -Path "web.config.server" -Destination "\\192.168.1.112\c$\inetpub\consulting.web\web.config" -Force
```

#### **B. Sul server, eseguire:**
```powershell
# Connettiti al server e esegui:
cd C:\inetpub\consulting.web
Stop-WebAppPool -Name "consulting.web"
Start-WebAppPool -Name "consulting.web"
```

---

## ✅ **STEP 5: VERIFICA DEPLOY**

### **5.1 Test immediato:**
```powershell
# Test dal PC di sviluppo
Invoke-WebRequest -Uri "http://192.168.1.112" -UseBasicParsing | Select-Object StatusCode
```

### **5.2 Test login:**
- Apri browser: `http://192.168.1.112`
- Login: `admin` / `123456`

---

## 🛠️ **COMANDI RAPIDI PER DEPLOY FUTURO**

### **Deploy completo in 4 comandi:**
```powershell
# 1. Naviga nella cartella
cd C:\dev\prova

# 2. Publish aggiornato
Remove-Item -Recurse -Force "publish" -ErrorAction SilentlyContinue
dotnet publish --configuration Release --output publish

# 3. Configura file server (eseguire solo la prima volta)
# [Già fatto se hai seguito la guida]

# 4. Deploy
.\DEPLOY_TO_SERVER.ps1
```

---

## 🚨 **TROUBLESHOOTING**

### **Se il sito non risponde:**
```powershell
# Sul server:
Get-IISAppPool -Name "consulting.web" | Select-Object Name, State
Start-WebAppPool -Name "consulting.web"
Get-EventLog -LogName Application -EntryType Error -Newest 3
```

### **Se ci sono errori di permessi:**
```powershell
# Sul server, riapplica permessi database:
sqlcmd -S "srv-dc\SQLEXPRESS" -E -Q "USE Consulting; ALTER ROLE db_datareader ADD MEMBER [IIS APPPOOL\consulting.web]; ALTER ROLE db_datawriter ADD MEMBER [IIS APPPOOL\consulting.web]"
```

### **Se il login non funziona:**
```powershell
# Sul server, controlla log:
Get-ChildItem "C:\inetpub\consulting.web\logs" | Sort-Object LastWriteTime -Descending | Select-Object -First 1 | Get-Content -Tail 20
```

---

## 📝 **RIEPILOGO COMANDI ESSENZIALI**

| **Operazione** | **Comando** |
|----------------|-------------|
| **Publish** | `dotnet publish --configuration Release --output publish` |
| **Deploy** | `.\DEPLOY_TO_SERVER.ps1` |
| **Verifica** | `Invoke-WebRequest -Uri "http://192.168.1.112"` |
| **Restart** | `Stop-WebAppPool -Name "consulting.web"; Start-WebAppPool -Name "consulting.web"` |

---

**🎯 Con questa guida puoi rifare il deploy infinite volte mantenendo la stessa configurazione funzionante!**




