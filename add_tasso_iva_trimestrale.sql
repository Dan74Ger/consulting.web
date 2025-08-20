-- ===================================================
-- AGGIUNTA CAMPO: Tasso per IVA Trimestrale
-- Default: 1% = 0.01
-- ===================================================

-- Verifica se la colonna esiste già
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'clienti' AND COLUMN_NAME = 'tasso_iva_trimestrale')
BEGIN
    -- Aggiunge la colonna con default
    ALTER TABLE clienti 
    ADD tasso_iva_trimestrale DECIMAL(5,4) DEFAULT 0.01 NULL;
    
    PRINT 'Campo tasso_iva_trimestrale aggiunto con successo alla tabella clienti';
    
    -- Aggiorna i record esistenti con il valore default (solo se appena creata)
    UPDATE clienti 
    SET tasso_iva_trimestrale = 0.01 
    WHERE tasso_iva_trimestrale IS NULL;
    
    PRINT 'Aggiornamento record completato';
END
ELSE
BEGIN
    PRINT 'Campo tasso_iva_trimestrale esiste già nella tabella clienti';
    
    -- Solo aggiorna i NULL se la colonna esisteva già
    IF EXISTS (SELECT * FROM clienti WHERE tasso_iva_trimestrale IS NULL)
    BEGIN
        UPDATE clienti 
        SET tasso_iva_trimestrale = 0.01 
        WHERE tasso_iva_trimestrale IS NULL;
        
        PRINT 'Aggiornamento record NULL completato';
    END
    ELSE
    BEGIN
        PRINT 'Nessun record da aggiornare';
    END
END
