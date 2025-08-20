-- ===================================================
-- AGGIUNTA CAMPI: IVA da versare con interessi
-- Per tutti i 4 trimestri
-- ===================================================

-- Verifica se le colonne esistono già
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'contabilita_interna_trimestrale' AND COLUMN_NAME = 'primo_trimestre_iva_versare')
BEGIN
    -- Aggiunge le colonne per tutti i trimestri
    ALTER TABLE contabilita_interna_trimestrale 
    ADD primo_trimestre_iva_versare DECIMAL(10,2) NULL DEFAULT 0;
    
    ALTER TABLE contabilita_interna_trimestrale 
    ADD secondo_trimestre_iva_versare DECIMAL(10,2) NULL DEFAULT 0;
    
    ALTER TABLE contabilita_interna_trimestrale 
    ADD terzo_trimestre_iva_versare DECIMAL(10,2) NULL DEFAULT 0;
    
    ALTER TABLE contabilita_interna_trimestrale 
    ADD quarto_trimestre_iva_versare DECIMAL(10,2) NULL DEFAULT 0;
    
    PRINT 'Campi IVA da versare con interessi aggiunti con successo alla tabella contabilita_interna_trimestrale';
END
ELSE
BEGIN
    PRINT 'Campi IVA da versare con interessi esistono già nella tabella contabilita_interna_trimestrale';
END
