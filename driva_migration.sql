BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250815163553_CreateAttivitaIrapTable_Empty', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [attivita_cu] (
    [id_attivita_cu] int NOT NULL IDENTITY,
    [id_anno] int NOT NULL,
    [id_cliente] int NOT NULL,
    [id_professionista] int NULL,
    [cu_lav_autonomo] bit NOT NULL,
    [cu_utili] bit NOT NULL,
    [invio_cu] bit NOT NULL,
    [num_cu] int NULL,
    [ricevuta_cu] bit NOT NULL,
    [invio_cliente_mail] bit NOT NULL,
    [invio_cliente_mail_data] datetime2 NULL,
    [note] nvarchar(500) NULL,
    [created_at] datetime2 NOT NULL,
    [updated_at] datetime2 NOT NULL,
    CONSTRAINT [PK_attivita_cu] PRIMARY KEY ([id_attivita_cu]),
    CONSTRAINT [FK_attivita_cu_anni_fiscali_id_anno] FOREIGN KEY ([id_anno]) REFERENCES [anni_fiscali] ([id_anno]) ON DELETE CASCADE,
    CONSTRAINT [FK_attivita_cu_clienti_id_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [clienti] ([id_cliente]) ON DELETE CASCADE,
    CONSTRAINT [FK_attivita_cu_professionisti_id_professionista] FOREIGN KEY ([id_professionista]) REFERENCES [professionisti] ([id_professionista])
);
GO

CREATE TABLE [attivita_driva] (
    [id_attivita_driva] int NOT NULL IDENTITY,
    [id_anno] int NOT NULL,
    [id_cliente] int NOT NULL,
    [id_professionista] int NULL,
    [codice_dr_iva] nvarchar(20) NULL,
    [appuntamento_data_ora] datetime2 NULL,
    [acconto_iva_tipo] nvarchar(20) NULL,
    [acconto_iva_credito_debito] nvarchar(10) NOT NULL,
    [importo_acconto_iva] decimal(18,2) NULL,
    [f24_acconto_iva_stato] nvarchar(20) NOT NULL,
    [raccolta_documenti] nvarchar(20) NOT NULL,
    [driva_inserita] bit NOT NULL,
    [driva_inserita_data] datetime2 NULL,
    [driva_controllata] bit NOT NULL,
    [driva_controllata_data] datetime2 NULL,
    [driva_credito_debito] nvarchar(10) NOT NULL,
    [importo_dr_iva] decimal(18,2) NULL,
    [f24_driva_consegnato] bit NOT NULL,
    [f24_driva_data] datetime2 NULL,
    [dr_visto] bit NOT NULL,
    [ricevuta_driva] bit NOT NULL,
    [driva_spedita] bit NOT NULL,
    [driva_spedita_data] datetime2 NULL,
    [tcg_data] datetime2 NULL,
    [note] nvarchar(500) NULL,
    [created_at] datetime2 NOT NULL,
    [updated_at] datetime2 NOT NULL,
    CONSTRAINT [PK_attivita_driva] PRIMARY KEY ([id_attivita_driva]),
    CONSTRAINT [FK_attivita_driva_anni_fiscali_id_anno] FOREIGN KEY ([id_anno]) REFERENCES [anni_fiscali] ([id_anno]) ON DELETE CASCADE,
    CONSTRAINT [FK_attivita_driva_clienti_id_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [clienti] ([id_cliente]) ON DELETE CASCADE,
    CONSTRAINT [FK_attivita_driva_professionisti_id_professionista] FOREIGN KEY ([id_professionista]) REFERENCES [professionisti] ([id_professionista])
);
GO

CREATE INDEX [IX_attivita_cu_id_anno] ON [attivita_cu] ([id_anno]);
GO

CREATE INDEX [IX_attivita_cu_id_cliente] ON [attivita_cu] ([id_cliente]);
GO

CREATE INDEX [IX_attivita_cu_id_professionista] ON [attivita_cu] ([id_professionista]);
GO

CREATE INDEX [IX_attivita_driva_id_anno] ON [attivita_driva] ([id_anno]);
GO

CREATE INDEX [IX_attivita_driva_id_cliente] ON [attivita_driva] ([id_cliente]);
GO

CREATE INDEX [IX_attivita_driva_id_professionista] ON [attivita_driva] ([id_professionista]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250816155412_CreateAttivitaDrivaTable', N'8.0.0');
GO

COMMIT;
GO

