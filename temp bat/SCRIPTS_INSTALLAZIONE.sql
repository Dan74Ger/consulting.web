-- =====================================================
-- SCRIPT INSTALLAZIONE DATABASE - CONSULTING GROUP SRL
-- =====================================================

-- 1. CREAZIONE DATABASE
USE master
GO

-- Verifica se il database esiste già
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'ConsultingGroupDB')
BEGIN
    PRINT 'Database ConsultingGroupDB esiste già!'
END
ELSE
BEGIN
    CREATE DATABASE ConsultingGroupDB
    PRINT 'Database ConsultingGroupDB creato con successo!'
END
GO

-- 2. CREAZIONE LOGIN E UTENTE
USE master
GO

-- Creazione login per l'applicazione
IF NOT EXISTS(SELECT * FROM sys.server_principals WHERE name = 'consulting_user')
BEGIN
    CREATE LOGIN [consulting_user] WITH PASSWORD = 'Password_Sicura_123!', 
                                        DEFAULT_DATABASE = ConsultingGroupDB,
                                        CHECK_EXPIRATION = OFF,
                                        CHECK_POLICY = OFF
    PRINT 'Login consulting_user creato con successo!'
END
ELSE
BEGIN
    PRINT 'Login consulting_user esiste già!'
END
GO

-- Switch al database applicazione
USE ConsultingGroupDB
GO

-- Creazione utente nel database
IF NOT EXISTS(SELECT * FROM sys.database_principals WHERE name = 'consulting_user')
BEGIN
    CREATE USER [consulting_user] FOR LOGIN [consulting_user]
    PRINT 'Utente consulting_user creato nel database!'
END
ELSE
BEGIN
    PRINT 'Utente consulting_user esiste già nel database!'
END
GO

-- 3. ASSEGNAZIONE PERMESSI
ALTER ROLE [db_datareader] ADD MEMBER [consulting_user]
ALTER ROLE [db_datawriter] ADD MEMBER [consulting_user]
ALTER ROLE [db_ddladmin] ADD MEMBER [consulting_user]
PRINT 'Permessi assegnati a consulting_user!'
GO

-- 4. CREAZIONE TABELLE (Entity Framework le creerà automaticamente, ma ecco la struttura)
/*
Le seguenti tabelle verranno create automaticamente da Entity Framework:
- AspNetUsers (utenti)
- AspNetRoles (ruoli)
- AspNetUserRoles (relazione utenti-ruoli)
- UserPermissions (permessi personalizzati)
- Banche (dati bancari)
- CarteCredito (carte di credito)
- Utenze (utenze varie)
- Mail (account email)
- Cancelleria (fornitori cancelleria)
- UtentiPC (utenti computer)
- AltriDati (altri dati generali)
- Entratel (accessi Entratel)
- UtentiTS (utenti TeamSystem)
*/

-- 5. VERIFICA INSTALLAZIONE
SELECT 
    'Database creato correttamente' AS Status,
    DB_NAME() AS DatabaseName,
    USER_NAME() AS CurrentUser,
    @@VERSION AS SQLServerVersion
GO

-- 6. TEST CONNESSIONE
SELECT 
    'Test connessione OK' AS TestResult,
    GETDATE() AS TestDate,
    @@SERVERNAME AS ServerName
GO

PRINT '============================================='
PRINT 'INSTALLAZIONE DATABASE COMPLETATA!'
PRINT '============================================='
PRINT 'Database: ConsultingGroupDB'
PRINT 'Login: consulting_user'
PRINT 'Password: Password_Sicura_123!'
PRINT '============================================='
PRINT 'PROSSIMI PASSI:'
PRINT '1. Eseguire migration Entity Framework'
PRINT '2. Verificare connessione applicazione'
PRINT '3. Testare login utenti predefiniti'
PRINT '============================================='
