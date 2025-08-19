-- Script per aggiungere 'semestrale' al vincolo CHECK su tipo_proforma nella tabella proforma_generate
-- Database: Consulting su server IT15\SQLEXPRESS

USE Consulting;
GO

-- Elimina il vincolo CHECK esistente sulla tabella proforma_generate
ALTER TABLE proforma_generate DROP CONSTRAINT CK_proforma_generate_tipo_proforma;
GO

-- Ricrea il vincolo CHECK includendo 'semestrale' e 'bimestrale'
ALTER TABLE proforma_generate ADD CONSTRAINT CK_proforma_generate_tipo_proforma 
CHECK (tipo_proforma IN ('trimestrale', 'bimestrale', 'semestrale', 'mensile'));
GO

-- Verifica che il vincolo sia stato aggiornato correttamente
SELECT definition 
FROM sys.check_constraints 
WHERE name = 'CK_proforma_generate_tipo_proforma';
GO

PRINT 'Vincolo CHECK su proforma_generate aggiornato con successo. "semestrale" e "bimestrale" ora sono consentiti.';
GO
