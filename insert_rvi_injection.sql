-- Insert Machine
INSERT INTO Machines_RVI (Name, Description, IsActive, CreatedAt)
VALUES ('MESIN INJECTION', 'Injection Molding Machine', 1, GETDATE());

-- Get the Machine ID
DECLARE @MachineId INT = SCOPE_IDENTITY();

-- Insert Machine Numbers (Sample)
INSERT INTO MachineNumbers_RVI (MachineId, Number, Location, IsActive, CreatedAt)
VALUES 
(@MachineId, 'PV 001', 'Area RVI', 1, GETDATE()),
(@MachineId, 'PV 002', 'Area RVI', 1, GETDATE());

-- Insert Checklist Items
INSERT INTO ChecklistItems_RVI (MachineId, OrderNumber, DetailName, StandardDescription, IsActive, CreatedAt)
VALUES
(@MachineId, 1, 'Emergency Stop', 'Saat tombol ditekan, mesin akan berhenti', 1, GETDATE()),
(@MachineId, 2, 'Safety Light Curtain', 'Pastikan pergerakan Ejector & Mold berhenti saat light curtain aktif', 1, GETDATE()),
(@MachineId, 3, 'Molding', 'Kondisi tidak seret/macet & baut-baut pengikat tidak kendor (perhatian khusus)', 1, GETDATE()),
(@MachineId, 4, 'Ejector dan sliding part', 'Tidak seret, tidak ada kebocoran oli', 1, GETDATE()),
(@MachineId, 5, 'Safety Door', 'Saat pintu terbuka mesin berhenti dan alarm safety door', 1, GETDATE()),
(@MachineId, 6, 'Pressure Gauge Cooling Water', 'Jarum menunjuk zona hijau', 1, GETDATE()),
(@MachineId, 7, 'Level Cooling Water no.1 dan no.2', 'Posisi indikator didalam range merah (min) dan hijau (max) dan warna air jernih/bening', 1, GETDATE()),
(@MachineId, 8, 'Oil Heater Pump', 'Posisi indikator berada diantara strip merah (min) dan kuning/hitam (max)', 1, GETDATE()),
(@MachineId, 9, 'Area dalam mesin', 'Tidak ada rubber (perhatian khusus area sensor)', 1, GETDATE()),
(@MachineId, 10, 'Area luar mesin', 'Bersih (tidak ada cairan seperti oli, release agent / tidak ada rubber)', 1, GETDATE()),
(@MachineId, 11, 'Exhaust Fan cooler panel', 'Kipas berputar (indikator pita berputar / mengeluarkan udara)', 1, GETDATE()),
(@MachineId, 12, 'Hydraulic System', 'Tidak ada kebocoran Oli', 1, GETDATE()),
(@MachineId, 13, 'Instalasi angin dan air', 'Tidak ada kebocoran', 1, GETDATE());
