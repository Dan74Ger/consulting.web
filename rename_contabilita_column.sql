-- Script per rinominare la colonna 'Contabilita' in 'ContabilitaInternaTrimestrale'
-- nella tabella clienti

USE Consulting;
GO

-- Rinomina la colonna 'Contabilita' in 'ContabilitaInternaTrimestrale'
EXEC sp_rename 'clienti.Contabilita', 'ContabilitaInternaTrimestrale', 'COLUMN';
GO

-- Verifica la modifica
SELECT TOP 1 
    ContabilitaInternaTrimestrale,
    NomeCliente
FROM clienti
WHERE NomeCliente IS NOT NULL;
GO

PRINT 'Colonna rinominata con successo da "Contabilita" a "ContabilitaInternaTrimestrale"';
