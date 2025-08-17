-- Creazione tabella proforma_generate per memorizzare le proforma automatiche
-- Database: IT15\SQLEXPRESS - Consulting

USE [Consulting]

-- Verifica se la tabella esiste già
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[proforma_generate]') AND type in (N'U'))
BEGIN
    -- CREAZIONE TABELLA: proforma_generate
    CREATE TABLE proforma_generate (
        id_proforma INT IDENTITY(1,1) PRIMARY KEY,
        id_cliente INT NOT NULL,
        id_anno_fatturazione INT NOT NULL,
        
        -- Dati del mandato di riferimento
        data_mandato DATE NULL,
        importo_mandato_annuo DECIMAL(10,2) NULL,
        tipo_proforma NVARCHAR(20) NOT NULL CHECK (tipo_proforma IN ('trimestrale', 'mensile')),
        
        -- Dati della singola proforma
        numero_rata INT NOT NULL,  -- 1,2,3,4 per trimestrale; 1-12 per mensile
        data_scadenza DATE NOT NULL,
        importo_rata DECIMAL(10,2) NOT NULL,
        
        -- Stato della proforma
        inviata BIT DEFAULT 0,
        pagata BIT DEFAULT 0,
        data_invio DATE NULL,
        data_pagamento DATE NULL,
        note NVARCHAR(500) NULL,
        
        -- Campi di audit
        created_at DATETIME2 DEFAULT GETDATE(),
        updated_at DATETIME2 DEFAULT GETDATE(),
        
        -- Foreign keys
        FOREIGN KEY (id_cliente) REFERENCES clienti(id_cliente),
        FOREIGN KEY (id_anno_fatturazione) REFERENCES anni_fatturazione(id_anno_fatturazione)
    );

    -- Indici per performance
    CREATE INDEX IX_proforma_cliente ON proforma_generate(id_cliente);
    CREATE INDEX IX_proforma_anno ON proforma_generate(id_anno_fatturazione);
    CREATE INDEX IX_proforma_scadenza ON proforma_generate(data_scadenza);
    CREATE INDEX IX_proforma_stato ON proforma_generate(inviata, pagata);

    PRINT 'Tabella proforma_generate creata con successo!';
END
ELSE
BEGIN
    PRINT 'La tabella proforma_generate esiste già.';
END

PRINT 'Script completato con successo!';
PRINT 'Tabella: proforma_generate';
PRINT 'Database: Consulting su server IT15\SQLEXPRESS';
