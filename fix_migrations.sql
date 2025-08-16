-- Aggiungo manualmente le migration mancanti alla tabella delle migration history
-- per evitare errori durante l'avvio dell'applicazione

-- Marca come applicate le migration problematiche
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) 
VALUES 
('20250815163359_CreateAttivitaIrapTable', '8.0.0'),
('20250815163553_CreateAttivitaIrapTable_Empty', '8.0.0'),
('20250816155412_CreateAttivitaDrivaTable', '8.0.0');
