-- ===================================================
-- AGGIUNTA CAMPO: Tasso per IVA Trimestrale (SEMPLICE)
-- Default: 1% = 0.01
-- ===================================================

-- Verifica se la colonna esiste già
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'clienti' AND COLUMN_NAME = 'tasso_iva_trimestrale')
BEGIN
    -- Aggiunge la colonna con default NOT NULL
    ALTER TABLE clienti 
    ADD tasso_iva_trimestrale DECIMAL(5,4) NOT NULL DEFAULT 0.01;
    
    PRINT 'Campo tasso_iva_trimestrale aggiunto con successo alla tabella clienti con default 0.01';
END
ELSE
BEGIN
    PRINT 'Campo tasso_iva_trimestrale esiste già nella tabella clienti';
END
