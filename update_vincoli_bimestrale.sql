-- Aggiornamento vincoli CHECK per supportare periodicità bimestrale
-- Database: Consulting
-- Data: $(Get-Date)

USE Consulting;
GO

-- 1. Rimuovere e ricreare vincolo su tabella clienti
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK__clienti__proform__220B0B18')
BEGIN
    ALTER TABLE clienti DROP CONSTRAINT CK__clienti__proform__220B0B18;
    PRINT 'Vincolo clienti.proforma_tipo rimosso.';
END

-- Creare nuovo vincolo per clienti che include 'bimestrale'
ALTER TABLE clienti 
ADD CONSTRAINT CK_clienti_proforma_tipo 
CHECK (proforma_tipo IN ('trimestrale', 'bimestrale', 'mensile'));
PRINT 'Nuovo vincolo clienti.proforma_tipo creato con bimestrale.';

-- 2. Rimuovere e ricreare vincolo su tabella proforma_generate
IF EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK__proforma___tipo___24E777C3')
BEGIN
    ALTER TABLE proforma_generate DROP CONSTRAINT CK__proforma___tipo___24E777C3;
    PRINT 'Vincolo proforma_generate.tipo_proforma rimosso.';
END

-- Creare nuovo vincolo per proforma_generate che include 'bimestrale'
ALTER TABLE proforma_generate 
ADD CONSTRAINT CK_proforma_generate_tipo_proforma 
CHECK (tipo_proforma IN ('trimestrale', 'bimestrale', 'mensile'));
PRINT 'Nuovo vincolo proforma_generate.tipo_proforma creato con bimestrale.';

-- Verifica vincoli creati
SELECT 
    OBJECT_NAME(parent_object_id) AS table_name,
    name AS constraint_name,
    definition
FROM sys.check_constraints 
WHERE name LIKE '%proforma%' OR name LIKE '%tipo_proforma%';

PRINT 'Aggiornamento vincoli CHECK completato!';
