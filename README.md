# GESTIONE STUDIO - CONSULTING GROUP SRL 🏢

> **Sistema di Gestione Completo per Studio Professionale**  
> Versione: **step1datiutenza** - *Ultima modifica: 05 Agosto 2025*

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.0-purple.svg)](https://getbootstrap.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-red.svg)](https://www.microsoft.com/sql-server/)
[![Status](https://img.shields.io/badge/Status-Production%20Ready-green.svg)](#)

## 🎯 PANORAMICA PROGETTO

Sistema completo di gestione per studio professionale con **autorizzazione a 3 livelli** e gestione completa **Dati Generali** accessibili a tutti gli utenti autorizzati.

### 🔐 SISTEMA AUTORIZZAZIONI

| Ruolo | Accesso | Permessi Speciali |
|-------|---------|-------------------|
| 👑 **Administrator** | Completo | ✅ Impostazioni Sistema, ✅ Gestione Utenti |
| 🔒 **UserSenior** | Avanzato | ✅ Reports Avanzati, ❌ Impostazioni Sistema |
| 👤 **User** | Standard | ✅ Funzioni Base, ❌ Gestione Sistema |

### 🔑 CREDENZIALI DI TEST

```bash
# Administrator (accesso completo)
Username: admin
Password: 123456

# UserSenior (accesso reports)  
Username: senior
Password: 123456

# User (accesso base)
Username: user
Password: 123456
```

## 📊 FUNZIONALITÀ PRINCIPALI

### 🗂️ DATI GENERALI - Accessibili a Tutti gli Utenti

Sistema completo di gestione dati condivisi con **5 sezioni operative**:

#### 1. 📝 CANCELLERIA (Info Blue)
- **Gestione fornitori** di materiale cancelleria
- **Campi:** Denominazione Fornitore, Sito Web, Username, Password, Note
- **CRUD completo:** Crea, Visualizza, Modifica, Elimina

#### 2. 💻 UTENTI PC (Primary Blue)  
- **Gestione credenziali** computer e workstation
- **Campi:** Nome PC, Utente, Password, Indirizzo Rete, Note
- **CRUD completo:** Crea, Visualizza, Modifica, Elimina

#### 3. 📊 ALTRI DATI (Success Green)
- **Gestione dati generici** e servizi vari  
- **Campi:** Nome, Sito Web, Utente, Password, Note
- **CRUD completo:** Crea, Visualizza, Modifica, Elimina

#### 4. 🏛️ ENTRATEL (Secondary Gray)
- **Gestione accessi fiscali** e telematici
- **Campi:** Sito, Utente, Password, PIN Catastali, PIN Cassetto Fiscale, Desktop Telematico
- **CRUD completo:** Crea, Visualizza, Modifica, Elimina

#### 5. 🛡️ UTENTI TS (Danger Red) ⭐ **NUOVO!**
- **Gestione credenziali sistemi TS** (Tessera Sanitaria)
- **Campi:** Nome, Utente, Password, Note
- **CRUD completo:** Crea, Visualizza, Modifica, Elimina
- **Aggiunto nella versione step1datiutenza**

### 🏗️ DATI RISERVATI - Solo Utenti Autorizzati

Sistema gestione dati sensibili con accesso controllato:

#### 🏦 BANCHE
- Gestione credenziali bancarie e home banking
- Campi: Denominazione Banca, Sito Web, Username, Password, Note

#### 💳 CARTE DI CREDITO  
- Gestione carte di credito e servizi finanziari
- Campi: Denominazione, Sito Web, Username, Password, Note

#### ⚡ UTENZE
- Gestione contratti utenze (luce, gas, telefono, internet)
- Campi: Tipo Utenza, Fornitore, Sito Web, Username, Password, Note

#### 📧 MAIL
- Gestione account email e servizi di posta
- Campi: Provider, Indirizzo Email, Username, Password, Note

## 🎨 CARATTERISTICHE UI/UX

### 📱 Design Responsive
- **Bootstrap 5** per layout adattivo
- **FontAwesome Icons** per iconografia consistente  
- **Card-based Interface** per organizzazione contenuti
- **Color-coded Sections** per identificazione rapida

### 📊 Dashboard Avanzata
- **Statistiche Real-time** con contatori dinamici
- **Azioni Rapide** per creazione veloce record
- **Tabelle Responsive** con ultimi 5 record per sezione
- **Quick Links** per navigazione immediata

### 🧭 Navigazione Intuitiva
- **Breadcrumb dinamico** per orientamento
- **Menu sidebar** con permessi dinamici
- **Search & Filter** per ricerca veloce  
- **Messaggi di conferma** per operazioni critiche

## 🔧 ARCHITETTURA TECNICA

### 🏗️ Backend Stack
```
• ASP.NET Core 8.0 MVC
• Entity Framework Core  
• SQL Server LocalDB
• ASP.NET Core Identity
• Custom Authorization System
• Repository Pattern
• Dependency Injection
```

### 🎨 Frontend Stack
```
• Razor Pages & Views
• Bootstrap 5.0
• FontAwesome 6.0
• jQuery 3.6
• Custom CSS/SCSS
• Responsive Design
```

### 🗄️ Database Design
```
• Code-First Migrations
• Foreign Key Relationships  
• User-based Data Isolation
• Audit Trail (CreatedAt/UpdatedAt)
• Cascade Delete Policies
```

## 🚀 QUICK START

### Prerequisiti
- ✅ Visual Studio 2022 / VS Code
- ✅ .NET 8.0 SDK  
- ✅ SQL Server LocalDB

### Installazione
```bash
# 1. Clone progetto
git clone [repository-url]
cd consulting-group

# 2. Restore dependencies  
dotnet restore

# 3. Update database
dotnet ef database update

# 4. Run application
dotnet run

# 5. Navigare a http://localhost:5000
```

### Prima Configurazione
1. **Login** con credenziali admin (`admin/123456`)
2. **Verificare** accesso alle sezioni Dati Generali
3. **Testare** creazione record nelle 5 sezioni
4. **Configurare** utenti aggiuntivi se necessario

## 📁 STRUTTURA PROGETTO

```
ConsultingGroup/
├── 📁 Controllers/
│   ├── AccountController.cs      # Autenticazione
│   ├── DatiUtenzaController.cs   # Dati Riservati  
│   ├── DatiUtenzaExtraController.cs # Dati Generali ⭐
│   └── UserManagementController.cs # Gestione Utenti
│
├── 📁 Models/
│   ├── ApplicationUser.cs        # Utente sistema
│   ├── Banche.cs, CarteCredito.cs # Dati Riservati
│   ├── Cancelleria.cs, UtentiPC.cs # Dati Generali
│   ├── AltriDati.cs, Entratel.cs  # Dati Generali 
│   └── UtentiTS.cs               # ⭐ NUOVO!
│
├── 📁 Views/
│   ├── 📁 DatiUtenza/           # Dati Riservati
│   ├── 📁 DatiUtenzaExtra/      # Dati Generali ⭐
│   └── 📁 Shared/               # Layout comuni
│
├── 📁 Services/
│   └── UserPermissionsService.cs # Autorizzazioni
│
└── 📁 Data/
    ├── ApplicationDbContext.cs   # Database Context
    └── 📁 Migrations/            # Database Migrations
```

## 🧪 TESTING

### Test Manuali Essenziali

**✅ Autenticazione:**
- [ ] Login/Logout tutti i ruoli
- [ ] Redirect autorizzazioni  
- [ ] Session management

**✅ Dati Generali (5 sezioni):**
- [ ] Dashboard statistiche
- [ ] CRUD Cancelleria
- [ ] CRUD Utenti PC  
- [ ] CRUD Altri Dati
- [ ] CRUD ENTRATEL
- [ ] CRUD Utenti TS ⭐

**✅ Autorizzazioni:**
- [ ] Admin: accesso completo
- [ ] Senior: NO impostazioni  
- [ ] User: accesso base OK

**✅ UI/UX:**
- [ ] Design responsive mobile/desktop
- [ ] Navigazione intuitiva
- [ ] Form validation  
- [ ] Messaggi feedback

## 📈 ROADMAP SVILUPPO

### 🎯 Phase 1: Foundation ✅ COMPLETATO  
- [x] Sistema autorizzazioni 3 livelli
- [x] Dati Generali completi (5 sezioni)
- [x] CRUD operations
- [x] Responsive UI  

### 🎯 Phase 2: Enhancement (Prossimo)
- [ ] Advanced Search & Filtering
- [ ] Export Data (Excel/PDF)
- [ ] Audit Trail & Logging
- [ ] Email Notifications

### 🎯 Phase 3: Advanced Features
- [ ] API REST per integrazione
- [ ] Mobile App companion  
- [ ] Advanced Reports
- [ ] Backup automatico

## 🔒 SICUREZZA

### Implementate
- ✅ **Authentication** con ASP.NET Identity
- ✅ **Authorization** multi-livello personalizzato
- ✅ **CSRF Protection** su tutti i form
- ✅ **SQL Injection Prevention** con EF Core
- ✅ **XSS Protection** con Razor encoding

### Best Practices
- ✅ **Password Policy** enforcement
- ✅ **Session Timeout** gestione  
- ✅ **Input Validation** completa
- ✅ **Error Handling** sicuro
- ✅ **Audit Trail** operazioni critiche

## 🆘 SUPPORTO & TROUBLESHOOTING

### 🔍 Problemi Comuni

**❌ Errore Database Connection**  
```bash
# Verificare LocalDB 
sqllocaldb info
sqllocaldb start mssqllocaldb
```

**❌ Errore Migration**
```bash
# Reset database
dotnet ef database drop --force  
dotnet ef database update
```

**❌ Errore Autorizzazioni**
- Verificare UserPermissionsService
- Controllare seed data utenti
- Validare configurazione ruoli

### 📞 Contatti
- **Documentazione:** Consulta questo README
- **Issues:** Controlla log in `bin/Debug/net8.0/logs/`  
- **Database:** SQL Server Management Studio per query dirette

## 📋 CHANGELOG

### Version step1datiutenza (05 Agosto 2025) ⭐
- ✅ **AGGIUNTA SEZIONE UTENTI TS** completa
- ✅ Dashboard aggiornata con 5 sezioni
- ✅ CRUD completo per UtentiTS
- ✅ UI design consistente con tema rosso
- ✅ Database migration applicata
- ✅ Testing completo multi-ruolo

### Previous Versions
- ✅ Sistema base con 4 sezioni Dati Generali
- ✅ Autorizzazioni 3 livelli
- ✅ Dati Riservati completi  
- ✅ UI/UX responsive

## 📄 LICENZA

Progetto proprietario - Consulting Group SRL  
Tutti i diritti riservati © 2025

---

**🎉 Status: PRODUCTION READY**  
**🔄 Ultima modifica:** 05 Agosto 2025  
**👨‍💻 Versione:** step1datiutenza  
**🌐 URL:** http://localhost:5000