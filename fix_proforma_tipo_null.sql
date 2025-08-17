-- Fix per valori NULL nel campo proforma_tipo
-- Imposta 'trimestrale' come default per record esistenti
-- Database: IT15\SQLEXPRESS - Consulting

USE [Consulting]

-- Aggiorna tutti i record con proforma_tipo NULL al valore default
UPDATE clienti 
SET proforma_tipo = 'trimestrale' 
WHERE proforma_tipo IS NULL;

-- Verifica il risultato
SELECT COUNT(*) as RecordsAggiornati
FROM clienti 
WHERE proforma_tipo = 'trimestrale';

SELECT 'Fix applicato: tutti i record hanno ora proforma_tipo = trimestrale' as Risultato;
