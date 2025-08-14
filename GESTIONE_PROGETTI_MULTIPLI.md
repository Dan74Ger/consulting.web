# 🚀 GESTIONE PROGETTI MULTIPLI - COMMIT AUTOMATICI

## 🎯 **SCENARIO: NUOVO PROGETTO**

Quando inizi un **nuovo progetto**, hai **3 opzioni**:

---

## ⚡ **OPZIONE 1: SETUP AUTOMATICO COMPLETO (CONSIGLIATA)**

### 🛠️ **Uso: `setup_nuovo_progetto.bat`**

**Doppio click su:** `setup_nuovo_progetto.bat`

**Ti chiederà:**
- 📁 Percorso nuovo progetto
- 👤 Username GitHub  
- 📦 Nome repository
- ⏰ Frequenza commit (30min, hourly, daily)

**Risultato:** Setup completo automatico per il nuovo progetto!

---

## 📋 **OPZIONE 2: CLONA CONFIGURAZIONE ESISTENTE**

### 🛠️ **Uso: `clona_configurazione.bat`**

**Vantaggi:**
- ✅ Copia tutti gli script esistenti
- ✅ Mantiene configurazioni testate
- ✅ Setup veloce

**Dovrai poi:**
1. Modificare il repository in `auto_commit.ps1`
2. Eseguire setup nella nuova cartella

---

## 🔧 **OPZIONE 3: SETUP MANUALE**

### **Passi manuali:**

1. **Crea cartella progetto**
2. **Copia script da questo progetto**
3. **Modifica auto_commit.ps1:**
   ```powershell
   # Cambia il push per puntare al nuovo repo
   git push origin master
   ```
4. **Configura Git:**
   ```bash
   git init
   git remote add origin https://github.com/USER/NUOVO_REPO.git
   ```
5. **Setup Task Scheduler con nuovo nome**

---

## 📊 **GESTIONE PROGETTI MULTIPLI**

### 🎮 **Task Scheduler separati per progetto**

**Ogni progetto avrà il suo task:**
- `GitHubAutoCommit_ConsultingGroup` (questo progetto)
- `GitHubAutoCommit_NuovoProgetto` (nuovo progetto)
- `GitHubAutoCommit_AltroProgetto` (altro progetto)

### 📋 **Visualizza tutti i task:**
```cmd
schtasks /query /tn "GitHubAutoCommit*"
```

### 🛑 **Rimuovi task specifico:**
```cmd
schtasks /delete /tn "GitHubAutoCommit_NomeProgetto" /f
```

---

## 🎯 **ESEMPI PRATICI**

### 📱 **Nuovo progetto Mobile App:**
```
Percorso: C:\dev\mobile_app
Repository: https://github.com/Dan74Ger/mobile-app.git
Frequenza: 30min
Task: GitHubAutoCommit_mobile_app
```

### 🌐 **Nuovo progetto Website:**
```
Percorso: C:\dev\website
Repository: https://github.com/Dan74Ger/my-website.git  
Frequenza: hourly
Task: GitHubAutoCommit_website
```

---

## ⚙️ **CONFIGURAZIONE AVANZATA**

### 🔄 **Frequenze diverse per progetto:**
- **Progetto principale**: Ogni 15-30 minuti
- **Progetti secondari**: Ogni ora
- **Progetti sperimentali**: Giornaliero

### 📁 **Struttura consigliata:**
```
C:\dev\
├── progetto_principale\     (commit ogni 30min)
├── progetto_secondario\     (commit ogni ora)  
├── esperimenti\             (commit giornaliero)
└── backup_scripts\          (script condivisi)
```

---

## 🛡️ **BACKUP SCRIPTS CONDIVISI**

### 💡 **Strategia Smart:**

1. **Mantieni una cartella:** `C:\dev\backup_scripts\`
2. **Conserva gli script master** in questa cartella
3. **Copia script** in ogni nuovo progetto
4. **Personalizza** solo il repository e percorso

### 📦 **Script da mantenere:**
- `auto_commit.ps1` (personalizzare repository)
- `auto_commit.bat`
- `setup_auto_*.bat`
- `gestione_task.bat`

---

## 🎉 **VANTAGGI SISTEMA MULTIPLO**

✅ **Backup indipendenti** per ogni progetto  
✅ **Frequenze personalizzate** per esigenze diverse  
✅ **Repository separati** su GitHub  
✅ **Gestione centralizzata** tramite Task Scheduler  
✅ **Scalabilità infinita** - aggiungi tutti i progetti che vuoi!

---

🚀 **Il tuo sistema di backup automatico può gestire INFINITI progetti!**
