-- =====================================================
-- SCRIPT AGGIUNTA CAMPI IMPORTO CREDITO/DEBITO
-- Tabella: contabilita_interna_trimestrale
-- Server: IT15\SQLEXPRESS
-- Database: Consulting
-- =====================================================

USE [Consulting]
GO

-- Verifica se la tabella esiste
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND type in (N'U'))
BEGIN
    PRINT 'Aggiunta campi importo credito/debito alla tabella contabilita_interna_trimestrale...'
    
    -- PRIMO TRIMESTRE - Aggiungi importi credito/debito
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'primo_trimestre_importo_credito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [primo_trimestre_importo_credito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto primo_trimestre_importo_credito'
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'primo_trimestre_importo_debito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [primo_trimestre_importo_debito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto primo_trimestre_importo_debito'
    END
    
    -- SECONDO TRIMESTRE - Aggiungi importi credito/debito
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'secondo_trimestre_importo_credito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [secondo_trimestre_importo_credito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto secondo_trimestre_importo_credito'
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'secondo_trimestre_importo_debito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [secondo_trimestre_importo_debito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto secondo_trimestre_importo_debito'
    END
    
    -- TERZO TRIMESTRE - Aggiungi importi credito/debito
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'terzo_trimestre_importo_credito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [terzo_trimestre_importo_credito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto terzo_trimestre_importo_credito'
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'terzo_trimestre_importo_debito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [terzo_trimestre_importo_debito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto terzo_trimestre_importo_debito'
    END
    
    -- QUARTO TRIMESTRE - Aggiungi importi credito/debito
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'quarto_trimestre_importo_credito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [quarto_trimestre_importo_credito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto quarto_trimestre_importo_credito'
    END
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND name = 'quarto_trimestre_importo_debito')
    BEGIN
        ALTER TABLE [dbo].[contabilita_interna_trimestrale] 
        ADD [quarto_trimestre_importo_debito] DECIMAL(10,2) NULL;
        PRINT '✓ Aggiunto quarto_trimestre_importo_debito'
    END
    
    PRINT '✅ Aggiunta campi importo credito/debito completata!'
    
    -- Verifica finale
    SELECT COUNT(*) as NumeroColonne 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'contabilita_interna_trimestrale';
    
    PRINT 'Verifica completata!'
END
ELSE
BEGIN
    PRINT '❌ ERRORE: Tabella contabilita_interna_trimestrale non trovata!'
    PRINT 'Eseguire prima lo script create_contabilita_interna_trimestrale.sql'
END
GO
