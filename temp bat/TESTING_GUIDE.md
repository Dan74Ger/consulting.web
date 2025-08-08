# Guida al Test del Sistema di Autorizzazione

## Sistema Implementato

Il sistema di Gestione Studio - Consulting Group SRL è ora configurato con un sistema di autorizzazione a 3 livelli:

### Ruoli Disponibili

1. **Administrator** - Accesso completo
2. **UserSenior** - Accesso avanzato (tutto tranne gestione utenti)
3. **User** - Accesso base limitato

## Utenti di Test Creati

Il sistema crea automaticamente questi utenti di test:

### 1. Administrator
- **Username:** `admin`
- **Password:** `123456`
- **Accesso:** Completo a tutto il sistema
- **Funzionalità:** Può creare utenti, gestire tutto

### 2. Senior User
- **Username:** `senior`
- **Password:** `123456`
- **Accesso:** Avanzato (tutte le sezioni tranne gestione utenti)
- **Funzionalità:** Report avanzati, impostazioni sistema, dati utenza

### 3. Basic User
- **Username:** `user`
- **Password:** `123456`
- **Accesso:** Limitato alle funzioni base
- **Funzionalità:** Visualizzazione dati base, operazioni semplici

## Come Testare

### 1. Avvia l'Applicazione
```bash
dotnet run
```

### 2. Accedi al Sistema
Vai su: `http://localhost:5000` (o la porta indicata)

### 3. Test dei Ruoli

#### Test Administrator (`admin` / `123456`)
- ✅ Dovrebbe reindirizzare a Dashboard Admin
- ✅ Menu laterale mostra "Gestione Utenti"
- ✅ Può accedere a tutte le sezioni
- ✅ Badge "Administrator" rosso nel menu

#### Test Senior User (`senior` / `123456`)
- ✅ Dovrebbe reindirizzare a "Area Senior"
- ✅ Menu mostra "Reports Avanzati" e "Impostazioni"
- ✅ NON vede "Gestione Utenti"
- ✅ Badge "Senior User" blu nel menu
- ✅ Può accedere a Dati Utenza e Gestione Clienti

#### Test Basic User (`user` / `123456`)
- ✅ Dovrebbe reindirizzare a "Dashboard Utente"
- ✅ Menu mostra solo "Visualizza Dati" e "Operazioni Base"
- ✅ NON vede Dati Utenza, Gestione Utenti, Reports Avanzati
- ✅ Badge "Basic User" grigio nel menu
- ✅ Dashboard mostra controlli di accesso con sezioni bloccate

## Funzionalità da Verificare

### Navigation per Ruolo
- **Admin:** Dashboard Admin, Gestione Utenti, tutto il resto
- **Senior:** Area Senior, Reports Avanzati, Impostazioni, NO Gestione Utenti
- **User:** Dashboard Utente, Visualizza Dati, Operazioni Base

### Controlli di Accesso
1. Prova ad accedere a URL diretti:
   - `/UserManagement` (solo Admin)
   - `/DatiUtenza` (Admin e Senior, NO User)
   - `/Senior/Reports` (Senior e Admin, NO User)

### Redirect Logic
- Ogni ruolo viene reindirizzato alla sua pagina appropriata dopo il login
- Gli utenti già autenticati vengono reindirizzati correttamente

## Struttura delle Pagine

### Admin
- Dashboard con statistiche
- Gestione completa utenti
- Accesso a tutto

### Senior
- Pagina di benvenuto con panoramica
- Reports avanzati con metriche
- Impostazioni sistema (escluso gestione utenti)

### User
- Dashboard con controlli di accesso visuali
- Sezioni base per operazioni limitate
- Indicazioni chiare su cosa può/non può fare

## Test di Sicurezza

1. **Test di autorizzazione:** Prova ad accedere a URL non autorizzati
2. **Test di redirect:** Verifica che ogni utente vada nella sua area corretta
3. **Test di menu:** Controlla che il menu mostri solo le opzioni permesse
4. **Test di badge:** Verifica che il ruolo sia visualizzato correttamente

## Note Tecniche

- Sistema basato su ASP.NET Core Identity
- Autorizzazioni implementate tramite `[Authorize(Roles = "...")]`
- Database seeding automatico degli utenti di test
- Redirect personalizzato nel controller Account
- Menu dinamico basato sui ruoli dell'utente

## Problemi Noti da Verificare

- Controlla che le migrazioni del database siano applicate
- Verifica che tutti i CSS e JavaScript si caricino correttamente
- Testa la funzionalità di logout
- Controlla che le autorizzazioni impediscano accessi non autorizzati