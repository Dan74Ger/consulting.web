-- Aggiunge campo Acconto IVA al quarto trimestre
ALTER TABLE [contabilita_interna_trimestrale] 
ADD [quarto_trimestre_acconto_iva] DECIMAL(18,2) NULL;
