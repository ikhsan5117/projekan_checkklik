-- Sample Andon Data untuk Testing
-- Jalankan script ini di SQL Server Management Studio atau Azure Data Studio

USE [System_Checking_Harian];
GO

-- Insert sample machines untuk MOLDED plant
DECLARE @PlantId INT = (SELECT Id FROM Plants WHERE PlantCode = 'MOLDED');

-- Insert machines jika belum ada
IF NOT EXISTS (SELECT 1 FROM AndonMachines WHERE PlantId = @PlantId AND MachineCode = 'Machine 1')
BEGIN
    INSERT INTO AndonMachines (PlantId, MachineCode, MachineName, IsActive, CreatedAt)
    VALUES (@PlantId, 'Machine 1', 'Injection Machine 1', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM AndonMachines WHERE PlantId = @PlantId AND MachineCode = 'Machine 2')
BEGIN
    INSERT INTO AndonMachines (PlantId, MachineCode, MachineName, IsActive, CreatedAt)
    VALUES (@PlantId, 'Machine 2', 'Press Machine 2', 1, GETDATE());
END

IF NOT EXISTS (SELECT 1 FROM AndonMachines WHERE PlantId = @PlantId AND MachineCode = 'Machine 9')
BEGIN
    INSERT INTO AndonMachines (PlantId, MachineCode, MachineName, IsActive, CreatedAt)
    VALUES (@PlantId, 'Machine 9', 'CNC Machine 9', 1, GETDATE());
END

-- Insert sample Andon records
DECLARE @Machine1Id INT = (SELECT Id FROM AndonMachines WHERE PlantId = @PlantId AND MachineCode = 'Machine 1');
DECLARE @Machine2Id INT = (SELECT Id FROM AndonMachines WHERE PlantId = @PlantId AND MachineCode = 'Machine 2');
DECLARE @Machine9Id INT = (SELECT Id FROM AndonMachines WHERE PlantId = @PlantId AND MachineCode = 'Machine 9');

DECLARE @LineStopId INT = (SELECT Id FROM StatusTypes WHERE StatusCode = 'LINE_STOP');
DECLARE @NoLoadingId INT = (SELECT Id FROM StatusTypes WHERE StatusCode = 'NO_LOADING');
DECLARE @RunningId INT = (SELECT Id FROM StatusTypes WHERE StatusCode = 'RUNNING');

DECLARE @MachineId INT = (SELECT Id FROM FourMCategories WHERE CategoryCode = 'MACHINE');
DECLARE @MaterialId INT = (SELECT Id FROM FourMCategories WHERE CategoryCode = 'MATERIAL');
DECLARE @NoProblemId INT = (SELECT Id FROM FourMCategories WHERE CategoryCode = 'NO_PROBLEM');

-- Sample Record 1: LINE STOP - MACHINE problem
INSERT INTO AndonRecords (PlantId, MachineId, StatusId, FourMCategoryId, Remark, RecordedAt, IsResolved, CreatedBy)
VALUES (@PlantId, @Machine1Id, @LineStopId, @MachineId, 'PROBLEM MESIN', GETDATE(), 0, 'admin');

-- Sample Record 2: LINE STOP - MATERIAL problem
INSERT INTO AndonRecords (PlantId, MachineId, StatusId, FourMCategoryId, Remark, RecordedAt, IsResolved, CreatedBy)
VALUES (@PlantId, @Machine2Id, @LineStopId, @MaterialId, 'MATERIAL SHORTAGE', GETDATE(), 0, 'admin');

-- Sample Record 3: NO LOADING
INSERT INTO AndonRecords (PlantId, MachineId, StatusId, FourMCategoryId, Remark, RecordedAt, IsResolved, CreatedBy)
VALUES (@PlantId, @Machine9Id, @NoLoadingId, @NoProblemId, 'NO PROBLEM', GETDATE(), 0, 'admin');

PRINT 'Sample data inserted successfully!';
GO
