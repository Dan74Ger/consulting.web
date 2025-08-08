# ✅ CHECKLIST INSTALLAZIONE SERVER - CONSULTING GROUP SRL

## 📋 LISTA CONTROLLO PRE-INSTALLAZIONE

### ☐ 1. PREREQUISITI SOFTWARE
- ☐ Windows Server 2019/2022 o Windows 10/11
- ☐ IIS installato e configurato
- ☐ .NET 9.0 ASP.NET Core Runtime installato
- ☐ SQL Server 2019/2022 o SQL Server Express installato
- ☐ Accesso amministratore al server
- ☐ Accesso amministratore a SQL Server

### ☐ 2. FILES PREPARAZIONE
- ☐ Cartella `publish` presente e completa
- ☐ File `INSTALLAZIONE_SERVER.md` presente
- ☐ File `SCRIPTS_INSTALLAZIONE.sql` presente
- ☐ File `appsettings.Server.json` presente
- ☐ Script `install_server.bat` presente

---

## 🚀 PROCEDURA INSTALLAZIONE STEP-BY-STEP

### ☐ STEP 1: PREPARAZIONE DATABASE
1. ☐ Aprire SQL Server Management Studio
2. ☐ Connettersi al server SQL
3. ☐ Aprire file `SCRIPTS_INSTALLAZIONE.sql`
4. ☐ Eseguire lo script completo
5. ☐ Verificare creazione database `ConsultingGroupDB`
6. ☐ Verificare creazione login `consulting_user`
7. ☐ Testare connessione con nuovo utente

### ☐ STEP 2: INSTALLAZIONE APPLICAZIONE
1. ☐ Copiare tutti i file di installazione sul server
2. ☐ Aprire Prompt comandi come **Amministratore**
3. ☐ Navigare nella cartella con i file di installazione
4. ☐ Eseguire: `install_server.bat`
5. ☐ Verificare che lo script completi senza errori
6. ☐ Controllare che cartella `C:\inetpub\consulting.web` sia stata creata
7. ☐ Verificare presenza di tutti i file nell'applicazione

### ☐ STEP 3: CONFIGURAZIONE DATABASE
1. ☐ Aprire file: `C:\inetpub\consulting.web\appsettings.Production.json`
2. ☐ Verificare stringa di connessione:
   ```json
   "DefaultConnection": "Server=NOME_SERVER;Database=ConsultingGroupDB;User Id=consulting_user;Password=Password_Sicura_123!;TrustServerCertificate=true;MultipleActiveResultSets=true;"
   ```
3. ☐ Sostituire `NOME_SERVER` con nome del server SQL
4. ☐ Salvare il file

### ☐ STEP 4: ESECUZIONE MIGRATION
1. ☐ Aprire Prompt comandi come **Amministratore**
2. ☐ Navigare in: `cd C:\inetpub\consulting.web`
3. ☐ Eseguire: `dotnet ConsultingGroup.dll`
4. ☐ Verificare che l'applicazione parta (anche se poi va in errore)
5. ☐ Verificare che le tabelle siano state create nel database

### ☐ STEP 5: CONFIGURAZIONE IIS
1. ☐ Aprire **Gestione IIS**
2. ☐ Verificare che sito **ConsultingGroup** sia presente
3. ☐ Verificare binding: porta 80, path `C:\inetpub\consulting.web`
4. ☐ Controllare Pool applicazioni **ConsultingGroup**:
   - ☐ Versione .NET CLR: **Nessun codice gestito**
   - ☐ Modalità pipeline: **Integrata**
   - ☐ Identità: **ApplicationPoolIdentity**
5. ☐ Aggiungere variabile ambiente `ASPNETCORE_ENVIRONMENT = Production`

---

## 🧪 TEST POST-INSTALLAZIONE

### ☐ TEST 1: VERIFICA TECNICA
1. ☐ Eseguire: `check_server.bat`
2. ☐ Verificare che tutti i controlli siano ✓ (green)
3. ☐ Risolvere eventuali problemi ✗ (red)

### ☐ TEST 2: ACCESSO WEB
1. ☐ Aprire browser
2. ☐ Navigare su: `http://localhost`
3. ☐ Verificare che appaia la pagina di login
4. ☐ Non devono apparire errori 500 o 404

### ☐ TEST 3: LOGIN UTENTI
1. ☐ **Administrator**: `admin` / `123456`
   - ☐ Login riuscito
   - ☐ Accesso dashboard amministratore
   - ☐ Menu "Gestione Utenti" visibile
   - ☐ Menu "Dati Riservati" accessibile
   - ☐ Menu "Dati Generali" accessibile

2. ☐ **UserSenior**: `senior` / `123456`
   - ☐ Login riuscito
   - ☐ Accesso dashboard senior
   - ☐ Menu "Dati Riservati" accessibile
   - ☐ Menu "Dati Generali" accessibile
   - ☐ Menu "Gestione Utenti" NON visibile

3. ☐ **User**: `user` / `123456`
   - ☐ Login riuscito
   - ☐ Accesso dashboard utente
   - ☐ Menu "Dati Generali" accessibile
   - ☐ Menu "Dati Riservati" NON accessibile
   - ☐ Menu "Gestione Utenti" NON visibile

### ☐ TEST 4: FUNZIONALITÀ CRUD
1. ☐ **Dati Riservati** (admin/senior):
   - ☐ Banche: Visualizza, Crea, Modifica, Elimina
   - ☐ Carte Credito: Visualizza, Crea, Modifica, Elimina
   - ☐ Utenze: Visualizza, Crea, Modifica, Elimina
   - ☐ Mail: Visualizza, Crea, Modifica, Elimina

2. ☐ **Dati Generali** (tutti gli utenti autorizzati):
   - ☐ Cancelleria: Visualizza, Crea, Modifica, Elimina
   - ☐ Utenti PC: Visualizza, Crea, Modifica, Elimina
   - ☐ Altri Dati: Visualizza, Crea, Modifica, Elimina
   - ☐ Entratel: Visualizza, Crea, Modifica, Elimina
   - ☐ Utenti TS: Visualizza, Crea, Modifica, Elimina

3. ☐ **Gestione Utenti** (solo admin):
   - ☐ Lista utenti
   - ☐ Creazione nuovo utente
   - ☐ Modifica utente esistente
   - ☐ Gestione permessi

---

## 🔧 RISOLUZIONE PROBLEMI COMUNI

### ❌ Errore 500.30 - App failed to start
- ☐ Verificare installazione .NET 9.0 Runtime
- ☐ Controllare permessi cartella `C:\inetpub\consulting.web`
- ☐ Verificare file `web.config`
- ☐ Controllare log in Event Viewer

### ❌ Errore connessione database
- ☐ Verificare stringa connessione in `appsettings.Production.json`
- ☐ Testare connessione SQL Server Management Studio
- ☐ Verificare permessi utente `consulting_user`
- ☐ Controllare firewall SQL Server

### ❌ Pagine non trovate (404)
- ☐ Verificare presenza cartella `Views` completa
- ☐ Controllare permessi lettura IIS_IUSRS
- ☐ Verificare configurazione pool applicazioni

### ❌ Login non funziona
- ☐ Verificare che migration database sia completata
- ☐ Controllare tabelle AspNetUsers, AspNetRoles
- ☐ Verificare seeder utenti predefiniti

---

## ✅ CHECKLIST FINALE

### ☐ CONFIGURAZIONE SICUREZZA
- ☐ Cambiare password utenti predefiniti
- ☐ Configurare HTTPS (raccomandato)
- ☐ Configurare firewall
- ☐ Impostare backup automatico database

### ☐ MONITORAGGIO
- ☐ Configurare log monitoring
- ☐ Testare backup/restore
- ☐ Documentare procedure manutenzione

### ☐ CONSEGNA UTENTE
- ☐ Fornire credenziali utenti
- ☐ Consegnare documentazione
- ☐ Spiegare procedure base
- ☐ Pianificare training utenti

---

## 📞 CONTATTI SUPPORTO

**Per assistenza tecnica:**
- Documentazione: `INSTALLAZIONE_SERVER.md`
- Script verifica: `check_server.bat`
- Log applicazione: `C:\inetpub\logs\consulting\`
- Log IIS: `C:\inetpub\logs\LogFiles\`

---

**🎉 INSTALLAZIONE COMPLETATA CON SUCCESSO!**

**Data installazione**: _______________  
**Installato da**: ___________________  
**Testato da**: _____________________  
**Note**: __________________________
