-- =====================================================
-- SCRIPT CREAZIONE TABELLA CONTABILITÀ INTERNA TRIMESTRALE
-- Server: IT15\SQLEXPRESS
-- Database: Consulting
-- =====================================================

USE [Consulting]
GO

-- Verifica se la tabella esiste già
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[contabilita_interna_trimestrale]') AND type in (N'U'))
BEGIN
    PRINT 'La tabella contabilita_interna_trimestrale esiste già.'
END
ELSE
BEGIN
    PRINT 'Creazione tabella contabilita_interna_trimestrale...'
    
    -- TABELLA: contabilita_interna_trimestrale
    CREATE TABLE [dbo].[contabilita_interna_trimestrale] (
        [id_contabilita_interna_trimestrale] INT IDENTITY(1,1) PRIMARY KEY,
        [id_anno] INT NOT NULL,
        [id_cliente] INT NOT NULL,
        [codice_contabilita] NVARCHAR(50) NULL,
        
        -- Primo Trimestre
        [primo_trimestre_ultima_ft_vendita] NVARCHAR(50) NULL,
        [primo_trimestre_data_ft] DATE NULL,
        [primo_trimestre_liq_iva_importo] DECIMAL(10,2) NULL,
        [primo_trimestre_debito_credito] NVARCHAR(10) NULL,
        [primo_trimestre_f24_consegnato] NVARCHAR(20) NULL,
        
        -- Secondo Trimestre
        [secondo_trimestre_ultima_ft_vendita] NVARCHAR(50) NULL,
        [secondo_trimestre_data_ft] DATE NULL,
        [secondo_trimestre_liq_iva_importo] DECIMAL(10,2) NULL,
        [secondo_trimestre_debito_credito] NVARCHAR(10) NULL,
        [secondo_trimestre_f24_consegnato] NVARCHAR(20) NULL,
        
        -- Terzo Trimestre
        [terzo_trimestre_ultima_ft_vendita] NVARCHAR(50) NULL,
        [terzo_trimestre_data_ft] DATE NULL,
        [terzo_trimestre_liq_iva_importo] DECIMAL(10,2) NULL,
        [terzo_trimestre_debito_credito] NVARCHAR(10) NULL,
        [terzo_trimestre_f24_consegnato] NVARCHAR(20) NULL,
        
        -- Quarto Trimestre
        [quarto_trimestre_ultima_ft_vendita] NVARCHAR(50) NULL,
        [quarto_trimestre_data_ft] DATE NULL,
        [quarto_trimestre_liq_iva_importo] DECIMAL(10,2) NULL,
        [quarto_trimestre_debito_credito] NVARCHAR(10) NULL,
        [quarto_trimestre_f24_consegnato] NVARCHAR(20) NULL,
        
        [created_at] DATETIME2 DEFAULT GETDATE(),
        [updated_at] DATETIME2 DEFAULT GETDATE(),
        
        -- Vincoli di chiave esterna
        CONSTRAINT [FK_contabilita_interna_trimestrale_anni_fiscali] 
            FOREIGN KEY ([id_anno]) REFERENCES [dbo].[anni_fiscali]([id_anno]),
        CONSTRAINT [FK_contabilita_interna_trimestrale_clienti] 
            FOREIGN KEY ([id_cliente]) REFERENCES [dbo].[clienti]([id_cliente])
    );
    
    -- Indici per migliorare le performance
    CREATE INDEX [IX_contabilita_interna_trimestrale_anno] 
        ON [dbo].[contabilita_interna_trimestrale] ([id_anno]);
    
    CREATE INDEX [IX_contabilita_interna_trimestrale_cliente] 
        ON [dbo].[contabilita_interna_trimestrale] ([id_cliente]);
    
    -- Indice composto per la combinazione anno-cliente (dovrebbe essere unica)
    CREATE UNIQUE INDEX [IX_contabilita_interna_trimestrale_anno_cliente] 
        ON [dbo].[contabilita_interna_trimestrale] ([id_anno], [id_cliente]);
    
    PRINT 'Tabella contabilita_interna_trimestrale creata con successo!'
    PRINT 'Indici creati con successo!'
END
GO

-- Verifica finale
SELECT COUNT(*) as NumeroColonne 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'contabilita_interna_trimestrale';

PRINT 'Script completato!'
