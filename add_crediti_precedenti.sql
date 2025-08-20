-- =====================================================
-- AGGIUNTA CAMPI CREDITI PRECEDENTI
-- Tabella: contabilita_interna_trimestrale
-- =====================================================

USE Consulting;
GO

-- Aggiunta campo credito anno precedente per il 1° trimestre
ALTER TABLE contabilita_interna_trimestrale
ADD primo_trimestre_credito_anno_precedente DECIMAL(10,2) NULL;

-- Aggiunta campi credito trimestre precedente per 2°, 3°, 4° trimestre
ALTER TABLE contabilita_interna_trimestrale
ADD secondo_trimestre_credito_trimestre_precedente DECIMAL(10,2) NULL;

ALTER TABLE contabilita_interna_trimestrale
ADD terzo_trimestre_credito_trimestre_precedente DECIMAL(10,2) NULL;

ALTER TABLE contabilita_interna_trimestrale
ADD quarto_trimestre_credito_trimestre_precedente DECIMAL(10,2) NULL;

PRINT 'Aggiunte con successo le colonne per crediti precedenti:';
PRINT '- primo_trimestre_credito_anno_precedente';
PRINT '- secondo_trimestre_credito_trimestre_precedente';
PRINT '- terzo_trimestre_credito_trimestre_precedente';
PRINT '- quarto_trimestre_credito_trimestre_precedente';
GO
