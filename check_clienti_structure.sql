-- Script per verificare la struttura della tabella clienti
USE Consulting;
GO

PRINT 'Verifica struttura tabella clienti:'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'clienti'
    AND (COLUMN_NAME LIKE '%contabilit%' OR COLUMN_NAME LIKE '%periodo%')
ORDER BY ORDINAL_POSITION;

PRINT 'Tutte le colonne della tabella clienti:'
SELECT 
    COLUMN_NAME,
    DATA_TYPE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'clienti'
ORDER BY ORDINAL_POSITION;
