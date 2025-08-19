-- Script per aggiungere 'bimestrale' al vincolo CHECK su proforma_tipo
-- Database: Consulting su server IT15\SQLEXPRESS

USE Consulting;
GO

-- Elimina il vincolo CHECK esistente
ALTER TABLE clienti DROP CONSTRAINT CK_clienti_proforma_tipo;
GO

-- Ricrea il vincolo CHECK includendo 'bimestrale'
ALTER TABLE clienti ADD CONSTRAINT CK_clienti_proforma_tipo 
CHECK (proforma_tipo IN ('trimestrale', 'bimestrale', 'semestrale', 'mensile'));
GO

-- Verifica che il vincolo sia stato aggiornato correttamente
SELECT definition 
FROM sys.check_constraints 
WHERE name = 'CK_clienti_proforma_tipo';
GO

PRINT 'Vincolo CHECK aggiornato con successo. "bimestrale" ora è consentito.';
GO
