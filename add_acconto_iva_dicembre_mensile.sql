-- Aggiunge campo Acconto IVA solo a dicembre nella contabilita_interna_mensile
ALTER TABLE [contabilita_interna_mensile] 
ADD [dicembre_acconto_iva] DECIMAL(18,2) NULL;
