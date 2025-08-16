-- Creazione tabella attivita_driva nel database Consulting
-- Basata sulla struttura fornita dall'utente

CREATE TABLE [attivita_driva] (
    [id_attivita_driva] int NOT NULL IDENTITY,
    [id_anno] int NOT NULL,
    [id_cliente] int NOT NULL,
    [id_professionista] int NULL,
    [codice_dr_iva] nvarchar(20) NULL,
    [appuntamento_data_ora] datetime2 NULL,
    [acconto_iva_tipo] nvarchar(20) NULL,
    [acconto_iva_credito_debito] nvarchar(10) NOT NULL DEFAULT 'zero',
    [importo_acconto_iva] decimal(10,2) NULL,
    [f24_acconto_iva_stato] nvarchar(20) NOT NULL DEFAULT 'non_spedito',
    [raccolta_documenti] nvarchar(20) NOT NULL DEFAULT 'da_richiedere',
    [driva_inserita] bit NOT NULL DEFAULT 0,
    [driva_inserita_data] date NULL,
    [driva_controllata] bit NOT NULL DEFAULT 0,
    [driva_controllata_data] date NULL,
    [driva_credito_debito] nvarchar(10) NOT NULL DEFAULT 'zero',
    [importo_dr_iva] decimal(10,2) NULL,
    [f24_driva_consegnato] bit NOT NULL DEFAULT 0,
    [f24_driva_data] date NULL,
    [dr_visto] bit NOT NULL DEFAULT 0,
    [ricevuta_driva] bit NOT NULL DEFAULT 0,
    [driva_spedita] bit NOT NULL DEFAULT 0,
    [driva_spedita_data] date NULL,
    [tcg_data] date NULL,
    [note] nvarchar(500) NULL,
    [created_at] datetime2 NOT NULL DEFAULT GETDATE(),
    [updated_at] datetime2 NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT [PK_attivita_driva] PRIMARY KEY ([id_attivita_driva]),
    CONSTRAINT [FK_attivita_driva_anni_fiscali_id_anno] FOREIGN KEY ([id_anno]) REFERENCES [anni_fiscali] ([id_anno]) ON DELETE CASCADE,
    CONSTRAINT [FK_attivita_driva_clienti_id_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [clienti] ([id_cliente]) ON DELETE CASCADE,
    CONSTRAINT [FK_attivita_driva_professionisti_id_professionista] FOREIGN KEY ([id_professionista]) REFERENCES [professionisti] ([id_professionista])
);

-- Creazione indici per performance
CREATE INDEX [IX_attivita_driva_id_anno] ON [attivita_driva] ([id_anno]);
CREATE INDEX [IX_attivita_driva_id_cliente] ON [attivita_driva] ([id_cliente]);
CREATE INDEX [IX_attivita_driva_id_professionista] ON [attivita_driva] ([id_professionista]);

-- Aggiunta della migration alla history per evitare conflitti
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
VALUES ('20250816155412_CreateAttivitaDrivaTable', '8.0.0');
