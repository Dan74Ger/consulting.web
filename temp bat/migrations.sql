IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FirstName] nvarchar(100) NOT NULL,
    [LastName] nvarchar(100) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastLoginAt] datetime2 NULL,
    [IsActive] bit NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250801153157_InitialCreate', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [UserPermissions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [CanAccessGestioneClienti] bit NOT NULL,
    [CanAccessDatiUtenza] bit NOT NULL,
    [CanAccessReports] bit NOT NULL,
    [CanAccessViewBasicData] bit NOT NULL,
    [CanAccessAdvancedReports] bit NOT NULL,
    [CanAccessRestrictedArea] bit NOT NULL,
    [CanViewStatisticsInDashboard] bit NOT NULL,
    [CanViewPersonalReports] bit NOT NULL,
    [CanViewActivityHistory] bit NOT NULL,
    [CanViewAdminData] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [ModifiedBy] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_UserPermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserPermissions_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_UserPermissions_UserId] ON [UserPermissions] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250804080503_AddUserPermissions', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250804093954_CreateDatiUtenzaTables', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AltriDati] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Nome] nvarchar(200) NOT NULL,
    [SitoWeb] nvarchar(300) NULL,
    [Utente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AltriDati] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AltriDati_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Banche] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [IBAN] nvarchar(34) NOT NULL,
    [CodiceUtente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [Indirizzo] nvarchar(500) NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Banche] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Banche_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Cancelleria] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [DenominazioneFornitore] nvarchar(200) NOT NULL,
    [SitoWeb] nvarchar(300) NULL,
    [NomeUtente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Cancelleria] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Cancelleria_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CarteCredito] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [NumeroCarta] nvarchar(20) NOT NULL,
    [Intestazione] nvarchar(200) NOT NULL,
    [MeseScadenza] int NOT NULL,
    [AnnoScadenza] int NOT NULL,
    [PIN] nvarchar(10) NOT NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CarteCredito] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CarteCredito_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Entratel] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Sito] nvarchar(300) NOT NULL,
    [Utente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [PinDatiCatastali] nvarchar(50) NULL,
    [PinDelegheCassettoFiscale] nvarchar(50) NULL,
    [PinCompleto] nvarchar(50) NULL,
    [DesktopTelematicoUtente] nvarchar(100) NULL,
    [DesktopTelematicoPassword] nvarchar(200) NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Entratel] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Entratel_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Mail] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [IndirizzoMail] nvarchar(200) NOT NULL,
    [NomeUtente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Mail] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Mail_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UtentiPC] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [NomePC] nvarchar(100) NOT NULL,
    [Utente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [IndirizzoRete] nvarchar(100) NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UtentiPC] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UtentiPC_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Utenze] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [DenominazioneUtenza] nvarchar(200) NOT NULL,
    [SitoWeb] nvarchar(300) NULL,
    [NomeUtente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [Note] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Utenze] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Utenze_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AltriDati_UserId] ON [AltriDati] ([UserId]);
GO

CREATE INDEX [IX_Banche_UserId] ON [Banche] ([UserId]);
GO

CREATE INDEX [IX_Cancelleria_UserId] ON [Cancelleria] ([UserId]);
GO

CREATE INDEX [IX_CarteCredito_UserId] ON [CarteCredito] ([UserId]);
GO

CREATE INDEX [IX_Entratel_UserId] ON [Entratel] ([UserId]);
GO

CREATE INDEX [IX_Mail_UserId] ON [Mail] ([UserId]);
GO

CREATE INDEX [IX_UtentiPC_UserId] ON [UtentiPC] ([UserId]);
GO

CREATE INDEX [IX_Utenze_UserId] ON [Utenze] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250804094844_AddDatiUtenzaCompleteSystem', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Banche] ADD [NomeBanca] nvarchar(200) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250804101038_AddNomeBancaToBanche', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [UserPermissions] ADD [CanAccessDatiUtenzaGenerale] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [UserPermissions] ADD [CanAccessDatiUtenzaRiservata] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250804115425_SplitDatiUtenzaPermissions', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanAccessAdvancedReports');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanAccessAdvancedReports];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanAccessDatiUtenza');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanAccessDatiUtenza];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanAccessReports');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanAccessReports];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanAccessRestrictedArea');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanAccessRestrictedArea];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanAccessViewBasicData');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanAccessViewBasicData];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanViewActivityHistory');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanViewActivityHistory];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanViewAdminData');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanViewAdminData];
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanViewPersonalReports');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanViewPersonalReports];
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[UserPermissions]') AND [c].[name] = N'CanViewStatisticsInDashboard');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [UserPermissions] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [UserPermissions] DROP COLUMN [CanViewStatisticsInDashboard];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250804123751_UpdateExistingUserPermissions', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [UtentiTS] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(200) NOT NULL,
    [Utente] nvarchar(100) NOT NULL,
    [Password] nvarchar(200) NOT NULL,
    [Note] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [UserId] nvarchar(450) NULL,
    CONSTRAINT [PK_UtentiTS] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UtentiTS_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_UtentiTS_UserId] ON [UtentiTS] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250804163739_AddUtentiTSTable', N'8.0.0');
GO

COMMIT;
GO

