-- Script per aggiungere la colonna CanAccessAreaAmministrativa alla tabella UserPermissions
-- Database: Consulting
-- Server: PCESTERNO-D\SQLEXPRESS

USE Consulting;
GO

-- Verifica se la colonna esiste già
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'UserPermissions' 
               AND COLUMN_NAME = 'CanAccessAreaAmministrativa')
BEGIN
    -- Aggiungi la colonna CanAccessAreaAmministrativa
    ALTER TABLE UserPermissions 
    ADD CanAccessAreaAmministrativa BIT NOT NULL DEFAULT 0;
    
    PRINT 'Colonna CanAccessAreaAmministrativa aggiunta con successo alla tabella UserPermissions';
    
    -- Aggiorna i permessi esistenti per gli utenti Senior (UserSenior)
    UPDATE UserPermissions 
    SET CanAccessAreaAmministrativa = 1
    FROM UserPermissions up
    INNER JOIN AspNetUsers u ON up.UserId = u.Id
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE r.Name = 'UserSenior';
    
    PRINT 'Permessi Area Amministrativa aggiornati per gli utenti Senior';
END
ELSE
BEGIN
    PRINT 'La colonna CanAccessAreaAmministrativa esiste già nella tabella UserPermissions';
END

-- Verifica il risultato
SELECT 
    u.UserName,
    r.Name as Role,
    up.CanAccessGestioneClienti,
    up.CanAccessDatiUtenzaRiservata,
    up.CanAccessDatiUtenzaGenerale,
    up.CanAccessAreaAmministrativa
FROM UserPermissions up
INNER JOIN AspNetUsers u ON up.UserId = u.Id
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
ORDER BY r.Name, u.UserName;

GO
