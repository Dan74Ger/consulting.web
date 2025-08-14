-- Script per correggere le date degli anni fiscali con le scadenze standard italiane
-- Eseguire questo script per aggiornare le date esistenti

-- Anno 2024 (Anno Corrente)
UPDATE anni_fiscali 
SET 
    scadenza_730 = '2024-07-31',           -- 31 luglio
    scadenza_740 = '2024-09-30',           -- 30 settembre  
    scadenza_750 = '2025-02-28',           -- 28 febbraio anno successivo
    scadenza_760 = '2024-10-31',           -- 31 ottobre
    scadenza_770 = '2024-11-30',           -- 30 novembre
    
    scadenza_ENC = '2024-07-31',           -- 31 luglio
    scadenza_IRAP = '2024-06-30',          -- 30 giugno
    scadenza_CU = '2025-02-28',            -- 28 febbraio anno successivo
    scadenza_DIVA = '2025-02-28',          -- 28 febbraio anno successivo (DRIVA)
    
    -- Lipe trimestrali
    scadenza_Lipe_1t = '2024-05-16',       -- 16 maggio
    scadenza_Lipe_2t = '2024-08-20',       -- 20 agosto  
    scadenza_Lipe_3t = '2024-11-18',       -- 18 novembre
    scadenza_Lipe_4t = '2025-02-17',       -- 17 febbraio anno successivo
    
    updated_at = GETDATE()
WHERE anno = 2024;

-- Anno 2023 (Anno Precedente)
UPDATE anni_fiscali 
SET 
    scadenza_730 = '2023-07-31',           -- 31 luglio
    scadenza_740 = '2023-09-30',           -- 30 settembre  
    scadenza_750 = '2024-02-28',           -- 28 febbraio anno successivo
    scadenza_760 = '2023-10-31',           -- 31 ottobre
    scadenza_770 = '2023-11-30',           -- 30 novembre
    
    scadenza_ENC = '2023-07-31',           -- 31 luglio
    scadenza_IRAP = '2023-06-30',          -- 30 giugno
    scadenza_CU = '2024-02-28',            -- 28 febbraio anno successivo
    scadenza_DIVA = '2024-02-28',          -- 28 febbraio anno successivo (DRIVA)
    
    -- Lipe trimestrali
    scadenza_Lipe_1t = '2023-05-16',       -- 16 maggio
    scadenza_Lipe_2t = '2023-08-20',       -- 20 agosto  
    scadenza_Lipe_3t = '2023-11-18',       -- 18 novembre
    scadenza_Lipe_4t = '2024-02-17',       -- 17 febbraio anno successivo
    
    updated_at = GETDATE()
WHERE anno = 2023;

-- Anno 2025 (Anno Futuro)
UPDATE anni_fiscali 
SET 
    scadenza_730 = '2025-07-31',           -- 31 luglio
    scadenza_740 = '2025-09-30',           -- 30 settembre  
    scadenza_750 = '2026-02-28',           -- 28 febbraio anno successivo
    scadenza_760 = '2025-10-31',           -- 31 ottobre
    scadenza_770 = '2025-11-30',           -- 30 novembre
    
    scadenza_ENC = '2025-07-31',           -- 31 luglio
    scadenza_IRAP = '2025-06-30',          -- 30 giugno
    scadenza_CU = '2026-02-28',            -- 28 febbraio anno successivo
    scadenza_DIVA = '2026-02-28',          -- 28 febbraio anno successivo (DRIVA)
    
    -- Lipe trimestrali
    scadenza_Lipe_1t = '2025-05-16',       -- 16 maggio
    scadenza_Lipe_2t = '2025-08-20',       -- 20 agosto  
    scadenza_Lipe_3t = '2025-11-18',       -- 18 novembre
    scadenza_Lipe_4t = '2026-02-17',       -- 17 febbraio anno successivo
    
    updated_at = GETDATE()
WHERE anno = 2025;

SELECT 'Date fiscali aggiornate correttamente!' as Risultato;

