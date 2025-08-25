-- Script per rinominare entrambe le colonne contabilità
-- nella tabella clienti

USE Consulting;
GO

PRINT 'Rinominazione colonne contabilità in corso...'

-- 1. Rinomina 'Contabilita' in 'contabilita_interna_trimestrale'
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('clienti') AND name = 'Contabilita')
BEGIN
    EXEC sp_rename 'clienti.Contabilita', 'contabilita_interna_trimestrale', 'COLUMN';
    PRINT '✅ Colonna Contabilita rinominata in contabilita_interna_trimestrale'
END
ELSE
BEGIN
    PRINT '⚠️ Colonna Contabilita non trovata (già rinominata?)'
END

-- 2. Rinomina 'periodo_contabilita' in 'contabilita_interna_mensile'  
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('clienti') AND name = 'periodo_contabilita')
BEGIN
    EXEC sp_rename 'clienti.periodo_contabilita', 'contabilita_interna_mensile', 'COLUMN';
    PRINT '✅ Colonna periodo_contabilita rinominata in contabilita_interna_mensile'
END
ELSE
BEGIN
    PRINT '⚠️ Colonna periodo_contabilita non trovata (già rinominata?)'
END

-- Verifica le modifiche
PRINT 'Verifica colonne rinominate:'
SELECT TOP 1 
    contabilita_interna_trimestrale,
    contabilita_interna_mensile,
    NomeCliente
FROM clienti
WHERE NomeCliente IS NOT NULL;

PRINT '✅ Rinominazione colonne completata con successo!';
