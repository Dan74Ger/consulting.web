-- Creazione tabella attivita_mod_tr_iva nel database Consulting
-- Modulo TRIVA - Trimestrali IVA
-- Database: IT15\SQLEXPRESS - Consulting

USE [Consulting]
GO

-- Verifica se la tabella esiste già
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[attivita_mod_tr_iva]') AND type in (N'U'))
BEGIN
    -- CREAZIONE TABELLA: attivita_mod_tr_iva
    CREATE TABLE attivita_mod_tr_iva (
        id_mod_tr_iva INT IDENTITY(1,1) PRIMARY KEY,
        id_anno INT NOT NULL,
        id_cliente INT NOT NULL,
        
        -- PRIMO TRIMESTRE
        primo_trimestre BIT DEFAULT 0,
        primo_trimestre_compilato BIT DEFAULT 0,
        primo_trimestre_spedito BIT DEFAULT 0,
        primo_trimestre_ricevuta BIT DEFAULT 0,
        primo_trimestre_mail_cliente BIT DEFAULT 0,
        
        -- SECONDO TRIMESTRE
        secondo_trimestre BIT DEFAULT 0,
        secondo_trimestre_compilato BIT DEFAULT 0,
        secondo_trimestre_spedito BIT DEFAULT 0,
        secondo_trimestre_ricevuta BIT DEFAULT 0,
        secondo_trimestre_mail_cliente BIT DEFAULT 0,
        
        -- TERZO TRIMESTRE
        terzo_trimestre BIT DEFAULT 0,
        terzo_trimestre_compilato BIT DEFAULT 0,
        terzo_trimestre_spedito BIT DEFAULT 0,
        terzo_trimestre_ricevuta BIT DEFAULT 0,
        terzo_trimestre_mail_cliente BIT DEFAULT 0,
        
        -- CAMPI DI AUDIT
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE(),
        
        -- FOREIGN KEYS
        FOREIGN KEY (id_anno) REFERENCES anni_fiscali(id_anno),
        FOREIGN KEY (id_cliente) REFERENCES clienti(id_cliente)
    );

    PRINT 'Tabella attivita_mod_tr_iva creata con successo!';
END
ELSE
BEGIN
    PRINT 'La tabella attivita_mod_tr_iva esiste già.';
END

-- Creazione indici per ottimizzare le query
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_attivita_mod_tr_iva_anno_cliente')
BEGIN
    CREATE INDEX IX_attivita_mod_tr_iva_anno_cliente 
    ON attivita_mod_tr_iva(id_anno, id_cliente);
    PRINT 'Indice IX_attivita_mod_tr_iva_anno_cliente creato.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_attivita_mod_tr_iva_anno')
BEGIN
    CREATE INDEX IX_attivita_mod_tr_iva_anno 
    ON attivita_mod_tr_iva(id_anno);
    PRINT 'Indice IX_attivita_mod_tr_iva_anno creato.';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_attivita_mod_tr_iva_cliente')
BEGIN
    CREATE INDEX IX_attivita_mod_tr_iva_cliente 
    ON attivita_mod_tr_iva(id_cliente);
    PRINT 'Indice IX_attivita_mod_tr_iva_cliente creato.';
END

PRINT 'Script completato con successo!';
PRINT 'Tabella: attivita_mod_tr_iva';
PRINT 'Database: Consulting su server IT15\SQLEXPRESS';
