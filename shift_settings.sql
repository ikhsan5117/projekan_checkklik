BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_BTR]'))
    SET IDENTITY_INSERT [Users_BTR] ON;
INSERT INTO [Users_BTR] ([Id], [CreatedAt], [Department], [Email], [FullName], [IsActive], [LastLogin], [Password], [Role], [Username])
VALUES (1, '2025-01-01T00:00:00.0000000', N'IT', N'admin.btr@amrvi.com', N'Admin BTR', CAST(1 AS bit), NULL, N'admin123', N'Admin', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_BTR]'))
    SET IDENTITY_INSERT [Users_BTR] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_HOSE]'))
    SET IDENTITY_INSERT [Users_HOSE] ON;
INSERT INTO [Users_HOSE] ([Id], [CreatedAt], [Department], [Email], [FullName], [IsActive], [LastLogin], [Password], [Role], [Username])
VALUES (1, '2025-01-01T00:00:00.0000000', N'IT', N'admin.hose@amrvi.com', N'Admin HOSE', CAST(1 AS bit), NULL, N'admin123', N'Admin', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_HOSE]'))
    SET IDENTITY_INSERT [Users_HOSE] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_MIXING]'))
    SET IDENTITY_INSERT [Users_MIXING] ON;
INSERT INTO [Users_MIXING] ([Id], [CreatedAt], [Department], [Email], [FullName], [IsActive], [LastLogin], [Password], [Role], [Username])
VALUES (1, '2025-01-01T00:00:00.0000000', N'IT', N'admin.mixing@amrvi.com', N'Admin MIXING', CAST(1 AS bit), NULL, N'admin123', N'Admin', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_MIXING]'))
    SET IDENTITY_INSERT [Users_MIXING] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_MOLDED]'))
    SET IDENTITY_INSERT [Users_MOLDED] ON;
INSERT INTO [Users_MOLDED] ([Id], [CreatedAt], [Department], [Email], [FullName], [IsActive], [LastLogin], [Password], [Role], [Username])
VALUES (1, '2025-01-01T00:00:00.0000000', N'IT', N'admin.molded@amrvi.com', N'Admin MOLDED', CAST(1 AS bit), NULL, N'admin123', N'Admin', N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'Department', N'Email', N'FullName', N'IsActive', N'LastLogin', N'Password', N'Role', N'Username') AND [object_id] = OBJECT_ID(N'[Users_MOLDED]'))
    SET IDENTITY_INSERT [Users_MOLDED] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260203152756_SeedDataPlants', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ShiftSettings] (
    [Id] int NOT NULL IDENTITY,
    [Plant] nvarchar(50) NOT NULL,
    [ShiftNumber] int NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ShiftSettings] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EndTime', N'Plant', N'ShiftNumber', N'StartTime', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[ShiftSettings]'))
    SET IDENTITY_INSERT [ShiftSettings] ON;
INSERT INTO [ShiftSettings] ([Id], [EndTime], [Plant], [ShiftNumber], [StartTime], [UpdatedAt])
VALUES (1, '14:00:00', N'RVI', 1, '05:00:00', '2026-02-05T10:01:10.3331815+07:00'),
(2, '22:00:00', N'RVI', 2, '14:00:00', '2026-02-05T10:01:10.3331820+07:00'),
(3, '05:00:00', N'RVI', 3, '22:00:00', '2026-02-05T10:01:10.3331821+07:00'),
(4, '14:00:00', N'BTR', 1, '05:00:00', '2026-02-05T10:01:10.3331832+07:00'),
(5, '22:00:00', N'BTR', 2, '14:00:00', '2026-02-05T10:01:10.3331832+07:00'),
(6, '05:00:00', N'BTR', 3, '22:00:00', '2026-02-05T10:01:10.3331833+07:00'),
(7, '14:00:00', N'HOSE', 1, '05:00:00', '2026-02-05T10:01:10.3331843+07:00'),
(8, '22:00:00', N'HOSE', 2, '14:00:00', '2026-02-05T10:01:10.3331843+07:00'),
(9, '05:00:00', N'HOSE', 3, '22:00:00', '2026-02-05T10:01:10.3331843+07:00'),
(10, '14:00:00', N'MOLDED', 1, '05:00:00', '2026-02-05T10:01:10.3331872+07:00'),
(11, '22:00:00', N'MOLDED', 2, '14:00:00', '2026-02-05T10:01:10.3331872+07:00'),
(12, '05:00:00', N'MOLDED', 3, '22:00:00', '2026-02-05T10:01:10.3331873+07:00'),
(13, '14:00:00', N'MIXING', 1, '05:00:00', '2026-02-05T10:01:10.3331882+07:00'),
(14, '22:00:00', N'MIXING', 2, '14:00:00', '2026-02-05T10:01:10.3331883+07:00'),
(15, '05:00:00', N'MIXING', 3, '22:00:00', '2026-02-05T10:01:10.3331884+07:00');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'EndTime', N'Plant', N'ShiftNumber', N'StartTime', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[ShiftSettings]'))
    SET IDENTITY_INSERT [ShiftSettings] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260205030111_AddShiftSettings', N'8.0.0');
GO

COMMIT;
GO

