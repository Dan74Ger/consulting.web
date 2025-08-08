-- BACKUP DATABASE STEP1DATIUTENZA - 05 Agosto 2025
-- Versione: step1datiutenza - Completamento sezione Dati Generali con UtentiTS
-- Database: Consulting (IT15\SQLEXPRESS)

BACKUP DATABASE [Consulting] 
TO DISK = N'C:\dev\prova\backups\step1datiutenza_05082025\Consulting_step1datiutenza_05082025.bak' 
WITH FORMAT, 
DESCRIPTION = N'step1datiutenza - Completamento sezione Dati Generali con UtentiTS per tutti i ruoli utente - 05/08/2025',
COMPRESSION;

-- Verifica backup
RESTORE VERIFYONLY 
FROM DISK = N'C:\dev\prova\backups\step1datiutenza_05082025\Consulting_step1datiutenza_05082025.bak';

-- Script per ripristino:
-- RESTORE DATABASE [Consulting_Restored] 
-- FROM DISK = N'C:\dev\prova\backups\step1datiutenza_05082025\Consulting_step1datiutenza_05082025.bak'
-- WITH REPLACE, 
-- MOVE 'Consulting' TO 'C:\Databases\Consulting_Restored.mdf',
-- MOVE 'Consulting_Log' TO 'C:\Databases\Consulting_Restored.ldf';