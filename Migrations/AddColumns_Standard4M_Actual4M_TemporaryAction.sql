-- SQL Script to add missing columns to HenkatenProblems table
-- Run this directly in your SQL Server database if migration cannot be run

USE [prd_checklist_machine]; -- Database name
GO

-- Add Standard4M column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HenkatenProblems]') AND name = 'Standard4M')
BEGIN
    ALTER TABLE [dbo].[HenkatenProblems]
    ADD [Standard4M] nvarchar(500) NULL;
    PRINT 'Column Standard4M added successfully';
END
ELSE
BEGIN
    PRINT 'Column Standard4M already exists';
END
GO

-- Add Actual4M column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HenkatenProblems]') AND name = 'Actual4M')
BEGIN
    ALTER TABLE [dbo].[HenkatenProblems]
    ADD [Actual4M] nvarchar(500) NULL;
    PRINT 'Column Actual4M added successfully';
END
ELSE
BEGIN
    PRINT 'Column Actual4M already exists';
END
GO

-- Add TemporaryAction column
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[HenkatenProblems]') AND name = 'TemporaryAction')
BEGIN
    ALTER TABLE [dbo].[HenkatenProblems]
    ADD [TemporaryAction] nvarchar(1000) NULL;
    PRINT 'Column TemporaryAction added successfully';
END
ELSE
BEGIN
    PRINT 'Column TemporaryAction already exists';
END
GO
