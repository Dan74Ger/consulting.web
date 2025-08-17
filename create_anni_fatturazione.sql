-- Creazione tabella anni_fatturazione nel database Consulting
-- Per gestire gli anni di fatturazione separatamente dagli anni fiscali
-- Database: IT15\SQLEXPRESS - Consulting

USE [Consulting]
GO

SET QUOTED_IDENTIFIER ON
GO

-- Verifica se la tabella esiste già
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[anni_fatturazione]') AND type in (N'U'))
BEGIN
    -- CREAZIONE TABELLA: anni_fatturazione
    CREATE TABLE anni_fatturazione (
        id_anno_fatturazione INT IDENTITY(1,1) PRIMARY KEY,
        anno INT NOT NULL UNIQUE,
        descrizione NVARCHAR(200) NULL,
        attivo BIT DEFAULT 1,
        anno_corrente BIT DEFAULT 0,
        note NVARCHAR(500) NULL,
        
        -- Campi di audit
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE()
    );

    -- Indice per performance
    CREATE INDEX IX_anni_fatturazione_anno ON anni_fatturazione(anno);
    CREATE INDEX IX_anni_fatturazione_attivo ON anni_fatturazione(attivo);
    
    -- Constraint per assicurare che solo un anno sia corrente
    CREATE UNIQUE INDEX IX_anni_fatturazione_anno_corrente 
    ON anni_fatturazione(anno_corrente) 
    WHERE anno_corrente = 1;

    PRINT 'Tabella anni_fatturazione creata con successo!';
    
    -- Inserisco alcuni anni di default
    INSERT INTO anni_fatturazione (anno, descrizione, attivo, anno_corrente) VALUES
    (2024, 'Anno Fatturazione 2024', 1, 0),
    (2025, 'Anno Fatturazione 2025', 1, 1),
    (2026, 'Anno Fatturazione 2026', 1, 0);
    
    PRINT 'Dati di esempio inseriti (2024, 2025, 2026)';
END
ELSE
BEGIN
    PRINT 'La tabella anni_fatturazione esiste già.';
END

PRINT 'Script completato con successo!';
PRINT 'Tabella: anni_fatturazione';
PRINT 'Database: Consulting su server IT15\SQLEXPRESS';
