-- Add Shift column to all inspection session tables
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InspectionSessions_RVI') AND name = 'Shift')
    ALTER TABLE InspectionSessions_RVI ADD Shift INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InspectionSessions_BTR') AND name = 'Shift')
    ALTER TABLE InspectionSessions_BTR ADD Shift INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InspectionSessions_HOSE') AND name = 'Shift')
    ALTER TABLE InspectionSessions_HOSE ADD Shift INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InspectionSessions_MOLDED') AND name = 'Shift')
    ALTER TABLE InspectionSessions_MOLDED ADD Shift INT NULL;

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('InspectionSessions_MIXING') AND name = 'Shift')
    ALTER TABLE InspectionSessions_MIXING ADD Shift INT NULL;
GO
