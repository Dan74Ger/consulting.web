-- Script per abilitare il permesso GestioneAttivita per tutti gli utenti esistenti
-- Da eseguire dopo l'aggiunta del nuovo permesso

-- Verifica lo stato attuale
SELECT u.UserName, u.Email, up.GestioneAttivita 
FROM AspNetUsers u
INNER JOIN UserPermissions up ON u.Id = up.UserId
ORDER BY u.UserName;

-- Aggiorna tutti gli utenti esistenti per abilitare GestioneAttivita
UPDATE UserPermissions 
SET GestioneAttivita = 1,
    UpdatedAt = GETDATE(),
    ModifiedBy = 'System_Migration'
WHERE GestioneAttivita = 0;

-- Verifica il risultato
SELECT u.UserName, u.Email, up.GestioneAttivita 
FROM AspNetUsers u
INNER JOIN UserPermissions up ON u.Id = up.UserId
ORDER BY u.UserName;

-- Mostra il conteggio degli utenti aggiornati
SELECT 
    COUNT(*) as TotalUsers,
    SUM(CASE WHEN GestioneAttivita = 1 THEN 1 ELSE 0 END) as UsersWithGestioneAttivita
FROM UserPermissions;
