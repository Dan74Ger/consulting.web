# 🔧 INSTALLAZIONE IIS MANUALE

## ❌ **PROBLEMA: Script in Loop**
Gli script automatici possono bloccarsi. Ecco come installare IIS manualmente.

---

## 🎯 **METODO 1: Pannello di Controllo (PIÙ SEMPLICE)**

1. **Aprire Pannello di Controllo**
   - `Windows + R` → `appwiz.cpl` → INVIO

2. **Attivazione/Disattivazione funzionalità Windows**
   - Cliccare su "Attiva o disattiva le funzionalità di Windows" (lato sinistro)

3. **Selezionare IIS**
   - ☑️ **Internet Information Services**
   - Espandere e selezionare:
     - ☑️ **Servizi Web** → **Funzionalità HTTP comuni**
     - ☑️ **Servizi Web** → **Sviluppo applicazioni** → **ASP.NET 4.8**
     - ☑️ **Strumenti di gestione Web** → **Console di gestione IIS**

4. **Cliccare OK**
   - Windows installerà IIS automaticamente
   - Attendere il completamento (può richiedere alcuni minuti)

5. **Riavviare se richiesto**

---

## 🎯 **METODO 2: PowerShell (RAPIDO)**

1. **Aprire PowerShell come Amministratore**
   - `Windows + X` → **Windows PowerShell (amministratore)**

2. **Eseguire comandi uno alla volta:**
   ```powershell
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment -All
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole -All
   ```

3. **Riavviare se richiesto**

---

## 🎯 **METODO 3: Comando Singolo**

1. **Aprire Prompt comandi come Amministratore**
   - `Windows + R` → `cmd` → `Ctrl + Shift + Invio`

2. **Eseguire:**
   ```cmd
   dism /online /enable-feature /featurename:IIS-WebServerRole /all /norestart
   ```

3. **Attendere completamento e riavviare**

---

## ✅ **VERIFICA INSTALLAZIONE**

Dopo l'installazione:

1. **Test browser:**
   ```
   http://localhost
   ```
   Dovrebbe apparire la **pagina di benvenuto IIS**

2. **Test comando:**
   ```cmd
   C:\Windows\system32\inetsrv\appcmd.exe list sites
   ```
   Dovrebbe mostrare il "Default Web Site"

3. **Test servizio:**
   ```cmd
   sc query W3SVC
   ```
   Dovrebbe mostrare "RUNNING"

---

## 🚀 **DOPO INSTALLAZIONE IIS**

1. **Scaricare .NET 9.0 Runtime:**
   - https://dotnet.microsoft.com/download/dotnet/9.0
   - **ASP.NET Core Runtime 9.0.x - Hosting Bundle**

2. **Installare .NET Runtime**

3. **Riavviare computer** (se non fatto)

4. **Eseguire installazione applicazione:**
   ```cmd
   INSTALL_SERVER_CONSULTING.bat
   ```

---

## 🆘 **SE CONTINUA A NON FUNZIONARE**

### **Windows 10 Home**
Windows 10 Home non include IIS. Serve **Windows 10 Pro** o superiore.

### **Verifica versione Windows:**
```cmd
winver
```

### **Alternative per Windows Home:**
- Usare **IIS Express** (più limitato)
- Usare **Kestrel** (server development)
- Aggiornare a Windows Pro

---

## 📞 **VERIFICA RAPIDA SISTEMA**

```cmd
# Verifica Windows
winver

# Verifica .NET
dotnet --info

# Verifica IIS (dopo installazione)
sc query W3SVC
```

---

**🎯 Il più semplice è il METODO 1 tramite Pannello di Controllo!**
