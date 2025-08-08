# 📋 GUIDA BACKUP - GESTIONE STUDIO CONSULTING GROUP

## 🎯 Contenuto Backup

### 📊 Database Backup (.bak)
- **Tipo**: SQL Server Full Backup
- **Database**: Consulting 
- **Include**: 
  - Tabelle utenti (AspNetUsers, AspNetRoles, AspNetUserRoles)
  - Tabella permessi personalizzati (UserPermissions)
  - Tutti i dati e configurazioni

### 📦 Codice Sorgente (.zip)
- **Include**:
  - Controllers (gestione MVC)
  - Models (entità database)
  - Views (interfaccia utente)
  - Services (logica business)
  - Migrations (versioning database)
  - File configurazione (appsettings.json, web.config)
  - Documentazione (README.md, TESTING_GUIDE.md)

- **Esclude**:
  - bin/ (file compilati)
  - obj/ (file temporanei)
  - .vs/ (file Visual Studio)
  - node_modules/ (dipendenze npm)

## 🚀 Come Utilizzare i Backup

### Ripristino Database
```sql
-- 1. Connettiti a SQL Server Management Studio
-- 2. Esegui questo comando:
RESTORE DATABASE [Consulting] 
FROM DISK = 'C:\dev\prova\backups\Consulting_DB_YYYYMMDD_HHMMSS.bak'
WITH REPLACE;
```

### Ripristino Codice Sorgente
1. Estrai il file ZIP in una nuova cartella
2. Apri il progetto in Visual Studio/VS Code
3. Esegui: `dotnet restore`
4. Aggiorna la connection string in appsettings.json se necessario
5. Esegui: `dotnet build`

## 🔄 Backup Automatico

### Script PowerShell
Esegui `create_backup.ps1` per creare nuovi backup:

```powershell
# Dalla cartella backups
.\create_backup.ps1

# Oppure specificando un percorso
.\create_backup.ps1 -BackupPath "D:\Backups"
```

### Frequenza Consigliata
- **Giornaliera**: Durante sviluppo attivo
- **Settimanale**: In produzione stabile
- **Prima di deploy**: Sempre prima di modifiche importanti

## 🛡️ Sicurezza

### Credenziali di Test nel Backup
Gli utenti di test inclusi nel backup:
- **admin/123456** - Administrator (accesso completo)
- **senior/123456** - UserSenior (permessi avanzati)
- **user/123456** - User (permessi base)

### ⚠️ IMPORTANTE
**Cambiare le password in produzione!**

## 📞 Supporto

In caso di problemi con il ripristino:
1. Verificare la connessione al database SQL Server
2. Controllare i permessi della cartella di destinazione  
3. Assicurarsi che .NET 8.0 SDK sia installato
4. Controllare la connection string in appsettings.json

---
**Data Backup**: $(Get-Date -Format "dd/MM/yyyy HH:mm:ss")
**Versione Progetto**: 2.0 - Sistema Permessi Personalizzati