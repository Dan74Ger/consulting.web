# 🛠️ RISOLUZIONE ERRORI SCRIPT COMMIT AUTOMATICI

## ❌ **PROBLEMA: "L'esecuzione di script è disabilitata"**

### 🚫 **Errore tipico:**
```
L'esecuzione di script è disabilitata nel sistema in uso
PSSecurityException: UnauthorizedAccess
```

---

## ✅ **SOLUZIONI (IN ORDINE DI FACILITÀ)**

### 🎯 **SOLUZIONE 1: USA IL FILE BATCH SICURO (CONSIGLIATA)**

**Doppio click su:** `setup_scheduler_safe.bat`

Questo file funziona **SEMPRE**, anche con policy restrittive!

### 🎯 **SOLUZIONE 2: FORZA L'ESECUZIONE**

**Doppio click su:** `setup_scheduler_force.bat`

Forza temporaneamente l'esecuzione degli script.

### 🎯 **SOLUZIONE 3: COMANDO MANUALE SICURO**

```powershell
# Apri PowerShell e esegui:
powershell -ExecutionPolicy Bypass -File "setup_scheduler.ps1"
```

### 🎯 **SOLUZIONE 4: CAMBIA POLICY (PERMANENTE)**

**⚠️ Solo se sei amministratore del sistema:**

```powershell
# Apri PowerShell come Amministratore
Set-ExecutionPolicy RemoteSigned

# Poi esegui normalmente:
.\setup_scheduler.ps1
```

---

## 🚀 **METODI ALTERNATIVI GARANTITI**

### 📅 **Setup Manuale Task Scheduler**

1. **Windows + R** → `taskschd.msc`
2. **Azioni** → **Crea attività di base**
3. **Nome**: `GitHubAutoCommit_ConsultingGroup`
4. **Trigger**: Giornaliero alle 18:00
5. **Azione**: Avvia programma
   - **Programma**: `powershell.exe`
   - **Argomenti**: `-ExecutionPolicy Bypass -File "C:\dev\prova\auto_commit.ps1"`
   - **Directory**: `C:\dev\prova`

### ⚡ **Commit Manuale Veloce**

**Doppio click su:** `auto_commit.bat`

Funziona sempre per commit immediati!

---

## 🔧 **ALTERNATIVE SEMPLIFICATE**

### 📝 **Creazione Task via CMD**

```cmd
schtasks /create /tn "GitHubAutoCommit" /tr "powershell.exe -ExecutionPolicy Bypass -File \"C:\dev\prova\auto_commit.ps1\"" /sc daily /st 18:00 /ru "%USERNAME%" /f
```

### 🖱️ **Collegamento Desktop**

Crea collegamento con destinazione:
```
powershell.exe -ExecutionPolicy Bypass -File "C:\dev\prova\auto_commit.ps1"
```

---

## 📊 **VERIFICA FUNZIONAMENTO**

### ✅ **Test Script Auto-Commit**
```cmd
powershell -ExecutionPolicy Bypass -File "C:\dev\prova\auto_commit.ps1"
```

### ✅ **Verifica Task Creato**
```cmd
schtasks /query /tn "GitHubAutoCommit*"
```

### ✅ **Esecuzione Manuale Task**
```cmd
schtasks /run /tn "GitHubAutoCommit_ConsultingGroup"
```

---

## 🎯 **RACCOMANDAZIONE FINALE**

**Per la massima compatibilità, usa sempre:**
1. `setup_scheduler_safe.bat` per setup
2. `auto_commit.bat` per commit manuali

Questi file funzionano su **qualsiasi sistema Windows** senza problemi di policy!

---

🎉 **Il tuo sistema di backup automatico sarà sempre attivo!**
