-- Creazione tabella anni_fatturazione nel database Consulting
USE [Consulting]

-- CREAZIONE TABELLA: anni_fatturazione
CREATE TABLE anni_fatturazione (
    id_anno_fatturazione INT IDENTITY(1,1) PRIMARY KEY,
    anno INT NOT NULL UNIQUE,
    descrizione NVARCHAR(200) NULL,
    attivo BIT DEFAULT 1,
    anno_corrente BIT DEFAULT 0,
    note NVARCHAR(500) NULL,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE()
);

-- Inserisco anni di default
INSERT INTO anni_fatturazione (anno, descrizione, attivo, anno_corrente) VALUES
(2024, 'Anno Fatturazione 2024', 1, 0),
(2025, 'Anno Fatturazione 2025', 1, 1),
(2026, 'Anno Fatturazione 2026', 1, 0);

SELECT 'Tabella anni_fatturazione creata e popolata con successo!' as Risultato;
