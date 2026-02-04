-- =============================================
-- Insert Injection Machine & Checklist for Plant MOLDED
-- Based on: CHECKLIST PERAWATAN MESIN HARIAN - PRODUKSI (PV 013)
-- =============================================

USE ChecklistAM_DB;
GO

-- 1. Insert Machine Category MOLDED if not exists
DECLARE @MachineId INT;
IF NOT EXISTS (SELECT 1 FROM dbo.Machines_MOLDED WHERE Name = 'Injection')
BEGIN
    INSERT INTO dbo.Machines_MOLDED (Name, Description, IsActive, CreatedAt)
    VALUES ('Injection', 'Injection / Jing Day - Taiwan', 1, GETDATE());
    SET @MachineId = SCOPE_IDENTITY();
END
ELSE
BEGIN
    SELECT @MachineId = Id FROM dbo.Machines_MOLDED WHERE Name = 'Injection';
END

-- 2. Insert Unit Number PV 013 for MOLDED
IF NOT EXISTS (SELECT 1 FROM dbo.MachineNumbers_MOLDED WHERE Number = 'PV 013' AND MachineId = @MachineId)
BEGIN
    INSERT INTO dbo.MachineNumbers_MOLDED (MachineId, Number, Location, IsActive, CreatedAt)
    VALUES (@MachineId, 'PV 013', 'Plant Molded', 1, GETDATE());
END

-- 3. Insert Checklist Items for Injection (MOLDED)
-- We'll clear existing ones if you want to re-insert, but better to check if exists.
-- For simplicity and precision, I'll use a merge-like approach or just insert if missing.

INSERT INTO dbo.ChecklistItems_MOLDED (MachineId, OrderNumber, DetailName, StandardDescription, IsActive, CreatedAt)
SELECT @MachineId, OrderNum, Detail, Std, 1, GETDATE()
FROM (
    VALUES 
    (1, 'Monitor/Touch Screen', 'Berfungsi'),
    (2, 'Push button & EMG stop', 'Berfungsi dan terpasang baik'),
    (3, 'Heater', 'Temperatur sesuai SOP'),
    (4, 'Sistem Hydraulic', 'Suara motor dan pompa tidak bising'),
    (5, 'Sistem Hydraulic', 'Level oli berada diatas garis Merah'),
    (6, 'Sistem Hydraulic', 'Suhu oli 30-55 C'),
    (7, 'Sistem Hydraulic', 'Tidak ada kebocoran oli'),
    (8, 'Cooling Water', 'Level diatas garis Merah'),
    (9, 'Cooling Water', 'Tidak ada kebocoran air'),
    (10, 'Thermal Oil System', 'Suara motor dan pompa tidak bising'),
    (11, 'Thermal Oil System', 'Tidak ada kebocoran oli'),
    (12, 'Thermal Oil System', 'Level oli cukup'),
    (13, 'Tie Bar', 'Bersih dari kotoran'),
    (14, 'Tie Bar', 'Lock tie bar Kencang'),
    (15, 'Screw', 'Putaran normal dan tidak bising'),
    (16, 'Screw', 'Baut Kencang'),
    (17, 'Installasi Angin', 'Tidak kebocoran angin di gun dan installasi'),
    (18, 'Installasi Air', 'Tidak kebocoran air di selang dan installasi'),
    (19, 'Kebersihan Mesin', 'Mesin dan area mesin bersih')
) AS Source(OrderNum, Detail, Std)
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.ChecklistItems_MOLDED 
    WHERE MachineId = @MachineId AND DetailName = Source.Detail AND StandardDescription = Source.Std
);

PRINT 'Data Injection PV 013 Berhasil Dimasukkan!';
GO
