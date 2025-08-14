-- ============================================
-- SETUP DATABASE CONSULTING - srv-dc\SQLEXPRESS
-- ============================================

USE master;
GO

-- Crea database se non esiste
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'Consulting')
BEGIN
    CREATE DATABASE [Consulting];
    PRINT 'Database Consulting creato con successo';
END
ELSE
BEGIN
    PRINT 'Database Consulting già esistente';
END
GO

-- Usa il database Consulting
USE [Consulting];
GO

-- Crea utente per l'applicazione (se necessario)
-- Nota: Con Windows Authentication non serve utente specifico
PRINT 'Database Consulting configurato per Windows Authentication';
GO

-- Verifica connessione
SELECT 
    DB_NAME() as 'Database Corrente',
    SUSER_SNAME() as 'Utente Corrente',
    GETDATE() as 'Data/Ora'
GO

PRINT 'Setup database completato con successo!';
PRINT 'Connection String: Server=srv-dc\SQLEXPRESS;Database=Consulting;Trusted_Connection=true;';




