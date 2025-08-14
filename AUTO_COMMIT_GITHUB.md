# 🤖 GUIDA COMMIT AUTOMATICI SU GITHUB

## 📋 COSA HO CREATO PER TE

Ho creato **3 script** per automatizzare i commit su GitHub:

### 📁 **File Creati**
- `auto_commit.ps1` - Script PowerShell principale
- `auto_commit.bat` - File batch per esecuzione facile
- `setup_scheduler.ps1` - Configurazione Task Scheduler
- `AUTO_COMMIT_GITHUB.md` - Questa guida

---

## 🚀 **OPZIONI DI UTILIZZO**

### ⚡ **OPZIONE 1: ESECUZIONE MANUALE**

**Doppio click su:** `auto_commit.bat`
- Esegue commit immediato se ci sono modifiche
- Mostra risultato in una finestra

**Da PowerShell:** 
```powershell
.\auto_commit.ps1
```

**Con messaggio personalizzato:**
```powershell
.\auto_commit.ps1 -CommitMessage "Aggiornamento importante del sistema"
```

### ⏰ **OPZIONE 2: SCHEDULAZIONE AUTOMATICA**

**Setup veloce (come Amministratore):**
```powershell
# Esecuzione giornaliera alle 18:00
.\setup_scheduler.ps1

# Esecuzione ogni ora
.\setup_scheduler.ps1 -Frequency "Hourly"

# Esecuzione settimanale alle 20:00
.\setup_scheduler.ps1 -Frequency "Weekly" -Time "20:00"
```

---

## 🎯 **MODALITÀ CONSIGLIATE**

### 🔥 **PER USO FREQUENTE**
- **Giornaliero alle 18:00**: `.\setup_scheduler.ps1`
- Perfetto per fine giornata lavorativa

### ⚡ **PER SVILUPPO INTENSO**
- **Ogni ora**: `.\setup_scheduler.ps1 -Frequency "Hourly"`
- Backup continuo durante lo sviluppo

### 🛡️ **PER SICUREZZA MASSIMA**
- **Settimanale**: `.\setup_scheduler.ps1 -Frequency "Weekly"`
- Backup regolare senza sovraccarico

---

## 📊 **COSA FA LO SCRIPT**

1. ✅ **Controllo modifiche** - Verifica se ci sono file cambiati
2. ➕ **Aggiunta file** - `git add .` automatico
3. 💾 **Commit** - Con timestamp automatico
4. 🚀 **Push GitHub** - Sincronizzazione immediata
5. 📝 **Log** - Salva cronologia in `auto_commit.log`
6. ❌ **Gestione errori** - Log errori in `auto_commit_errors.log`

---

## 🔧 **GESTIONE TASK SCHEDULER**

### Aprire Task Scheduler:
1. **Windows + R** → `taskschd.msc`
2. Cerca: `GitHubAutoCommit_ConsultingGroup`

### Modificare orario:
1. Tasto destro sul task → **Proprietà**
2. Tab **Trigger** → **Modifica**
3. Cambia orario/frequenza

### Disabilitare:
1. Tasto destro → **Disabilita**

### Rimuovere:
1. Tasto destro → **Elimina**

---

## 🛡️ **SICUREZZA**

- ✅ Lo script controlla sempre se ci sono modifiche prima del commit
- ✅ Non committterà se non ci sono cambiamenti
- ✅ Gestisce errori di connessione GitHub
- ✅ Mantiene log di tutte le operazioni
- ✅ Non sovrascrive mai il lavoro esistente

---

## 🎯 **ESEMPI PRATICI**

### Test immediato:
```bash
# Fai una piccola modifica al progetto
echo "# Test" >> test.txt

# Esegui commit manuale
.\auto_commit.bat
```

### Setup per ufficio (giornaliero):
```powershell
# Apri PowerShell come Amministratore
.\setup_scheduler.ps1 -Time "17:30"
```

### Setup per casa (settimanale):
```powershell
# Apri PowerShell come Amministratore  
.\setup_scheduler.ps1 -Frequency "Weekly" -Time "21:00"
```

---

## 📞 **SUPPORTO**

Se hai problemi:
1. Controlla i log: `auto_commit.log` e `auto_commit_errors.log`
2. Verifica connessione internet
3. Controlla che Git sia configurato correttamente
4. Assicurati di avere permessi su GitHub

---

🎉 **Il tuo codice sarà sempre al sicuro su GitHub!**
