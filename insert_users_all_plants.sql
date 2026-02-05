- =============================================
-- Insert Users for All Plants
-- Created: 2026-02-04
-- =============================================

USE ChecklistAM_DB;
GO

-- =============================================
-- RVI (Rubber Vibration Isolator) Users
-- =============================================
INSERT INTO dbo.Users_RVI (Username, FullName, Email, Password, Role, Department, IsActive, CreatedAt)
VALUES 
    ('AdminRVI', 'Admin RVI', 'admin.rvi@company.com', 'RVI123', 'Admin', 'RVI', 1, GETDATE()),
    ('UserRVI', 'User RVI', 'user.rvi@company.com', 'RVI123', 'User', 'RVI', 1, GETDATE());

-- =============================================
-- BTR (Bridgestone) Users
-- =============================================
INSERT INTO dbo.Users_BTR (Username, FullName, Email, Password, Role, Department, IsActive, CreatedAt)
VALUES 
    ('AdminBTR', 'Admin BTR', 'admin.btr@company.com', 'BTR123', 'Admin', 'BTR', 1, GETDATE()),
    ('UserBTR', 'User BTR', 'user.btr@company.com', 'BTR123', 'User', 'BTR', 1, GETDATE());

-- =============================================
-- HOSE Users
-- =============================================
INSERT INTO dbo.Users_HOSE (Username, FullName, Email, Password, Role, Department, IsActive, CreatedAt)
VALUES 
    ('AdminHOSE', 'Admin HOSE', 'admin.hose@company.com', 'HOSE123', 'Admin', 'HOSE', 1, GETDATE()),
    ('UserHOSE', 'User HOSE', 'user.hose@company.com', 'HOSE123', 'User', 'HOSE', 1, GETDATE());

-- =============================================
-- MOLDED Users
-- =============================================
INSERT INTO dbo.Users_MOLDED (Username, FullName, Email, Password, Role, Department, IsActive, CreatedAt)
VALUES 
    ('AdminMolded', 'Admin Molded', 'admin.molded@company.com', 'Molded123', 'Admin', 'MOLDED', 1, GETDATE()),
    ('UserMolded', 'User Molded', 'user.molded@company.com', 'Molded123', 'User', 'MOLDED', 1, GETDATE());

-- =============================================
-- MIXING Users
-- =============================================
INSERT INTO dbo.Users_MIXING (Username, FullName, Email, Password, Role, Department, IsActive, CreatedAt)
VALUES 
    ('AdminMixing', 'Admin Mixing', 'admin.mixing@company.com', 'Mixing123', 'Admin', 'MIXING', 1, GETDATE()),
    ('UserMixing', 'User Mixing', 'user.mixing@company.com', 'Mixing123', 'User', 'MIXING', 1, GETDATE());

GO

-- =============================================
-- Verify Inserted Data
-- =============================================
PRINT 'RVI Users:';
SELECT Username, FullName, Role, Department FROM dbo.Users_RVI;

PRINT 'BTR Users:';
SELECT Username, FullName, Role, Department FROM dbo.Users_BTR;

PRINT 'HOSE Users:';
SELECT Username, FullName, Role, Department FROM dbo.Users_HOSE;

PRINT 'MOLDED Users:';
SELECT Username, FullName, Role, Department FROM dbo.Users_MOLDED;

PRINT 'MIXING Users:';
SELECT Username, FullName, Role, Department FROM dbo.Users_MIXING;
