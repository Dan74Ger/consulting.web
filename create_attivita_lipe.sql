-- TABELLA: attivita_lipe
CREATE TABLE attivita_lipe (
    id_attivita_lipe INT IDENTITY(1,1) PRIMARY KEY,
    id_anno INT NOT NULL,
    id_cliente INT NOT NULL,
    id_professionista INT NULL,
    
    -- Primo Trimestre
    t1_raccolta_documenti NVARCHAR(20) DEFAULT 'da_richiedere',
    t1_lipe_inserita BIT DEFAULT 0,
    t1_lipe_inserita_data DATE NULL,
    t1_lipe_controllata BIT DEFAULT 0,
    t1_lipe_controllata_data DATE NULL,
    t1_lipe_spedita BIT DEFAULT 0,
    t1_lipe_spedita_data DATE NULL,
    t1_created_at DATETIME2 DEFAULT GETDATE(),
    t1_updated_at DATETIME2 DEFAULT GETDATE(),
    
    -- Secondo Trimestre
    t2_raccolta_documenti NVARCHAR(20) DEFAULT 'da_richiedere',
    t2_lipe_inserita BIT DEFAULT 0,
    t2_lipe_inserita_data DATE NULL,
    t2_lipe_controllata BIT DEFAULT 0,
    t2_lipe_controllata_data DATE NULL,
    t2_lipe_spedita BIT DEFAULT 0,
    t2_lipe_spedita_data DATE NULL,
    t2_created_at DATETIME2 DEFAULT GETDATE(),
    t2_updated_at DATETIME2 DEFAULT GETDATE(),
    
    -- Terzo Trimestre
    t3_raccolta_documenti NVARCHAR(20) DEFAULT 'da_richiedere',
    t3_lipe_inserita BIT DEFAULT 0,
    t3_lipe_inserita_data DATE NULL,
    t3_lipe_controllata BIT DEFAULT 0,
    t3_lipe_controllata_data DATE NULL,
    t3_lipe_spedita BIT DEFAULT 0,
    t3_lipe_spedita_data DATE NULL,
    t3_created_at DATETIME2 DEFAULT GETDATE(),
    t3_updated_at DATETIME2 DEFAULT GETDATE(),
    
    -- Quarto Trimestre
    t4_raccolta_documenti NVARCHAR(20) DEFAULT 'da_richiedere',
    t4_lipe_inserita BIT DEFAULT 0,
    t4_lipe_inserita_data DATE NULL,
    t4_lipe_controllata BIT DEFAULT 0,
    t4_lipe_controllata_data DATE NULL,
    t4_lipe_spedita BIT DEFAULT 0,
    t4_lipe_spedita_data DATE NULL,
    t4_created_at DATETIME2 DEFAULT GETDATE(),
    t4_updated_at DATETIME2 DEFAULT GETDATE(),
    
    FOREIGN KEY (id_anno) REFERENCES anni_fiscali(id_anno),
    FOREIGN KEY (id_cliente) REFERENCES clienti(id_cliente),
    FOREIGN KEY (id_professionista) REFERENCES professionisti(id_professionista)
);

-- Creazione indici per migliorare le performance
CREATE INDEX [IX_attivita_lipe_id_anno] ON [attivita_lipe] ([id_anno]);
CREATE INDEX [IX_attivita_lipe_id_cliente] ON [attivita_lipe] ([id_cliente]);
CREATE INDEX [IX_attivita_lipe_id_professionista] ON [attivita_lipe] ([id_professionista]);
