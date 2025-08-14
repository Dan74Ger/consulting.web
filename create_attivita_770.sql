-- Creazione tabella attivita_770 per MOD 770
USE Consulting;
GO

-- Verifica se la tabella esiste già
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'attivita_770')
BEGIN
    CREATE TABLE attivita_770 (
        id_attivita_770 INT IDENTITY(1,1) PRIMARY KEY,
        id_anno INT NOT NULL,
        id_cliente INT NOT NULL,
        id_professionista INT NULL,
        mod_770_lav_autonomo BIT DEFAULT 0,
        mod_770_ordinario BIT DEFAULT 0,
        inserimento_dati_dr BIT DEFAULT 0,
        dr_invio BIT DEFAULT 0,
        ricevuta BIT DEFAULT 0,
        pec_invio_dr BIT DEFAULT 0,
        mod_cu_fatte BIT DEFAULT 0,
        cu_utili_presenti BIT DEFAULT 0,
        note NVARCHAR(500) NULL,
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE(),
        
        -- Foreign Keys
        CONSTRAINT FK_attivita_770_anni_fiscali 
            FOREIGN KEY (id_anno) REFERENCES anni_fiscali(id_anno),
        CONSTRAINT FK_attivita_770_clienti 
            FOREIGN KEY (id_cliente) REFERENCES clienti(id_cliente),
        CONSTRAINT FK_attivita_770_professionisti 
            FOREIGN KEY (id_professionista) REFERENCES professionisti(id_professionista)
    );

    -- Crea gli indici
    CREATE INDEX IX_attivita_770_id_anno ON attivita_770(id_anno);
    CREATE INDEX IX_attivita_770_id_cliente ON attivita_770(id_cliente);
    CREATE INDEX IX_attivita_770_id_professionista ON attivita_770(id_professionista);

    PRINT 'Tabella attivita_770 creata con successo!';
END
ELSE
BEGIN
    PRINT 'Tabella attivita_770 esiste già.';
END
GO

-- Verifica della creazione
SELECT 
    TABLE_NAME, 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'attivita_770'
ORDER BY ORDINAL_POSITION;
GO
