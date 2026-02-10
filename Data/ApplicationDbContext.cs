using Microsoft.EntityFrameworkCore;
using AMRVI.Models;

namespace AMRVI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ================== RVI PLANT (Default / Current App) ==================
        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineNumber> MachineNumbers { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        public DbSet<InspectionSession> InspectionSessions { get; set; }
        public DbSet<InspectionResult> InspectionResults { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ShiftSetting> ShiftSettings { get; set; }

        // ================== BTR PLANT ==================
        public DbSet<Machine_BTR> Machines_BTR { get; set; }
        public DbSet<MachineNumber_BTR> MachineNumbers_BTR { get; set; }
        public DbSet<ChecklistItem_BTR> ChecklistItems_BTR { get; set; }
        public DbSet<InspectionSession_BTR> InspectionSessions_BTR { get; set; }
        public DbSet<InspectionResult_BTR> InspectionResults_BTR { get; set; }
        public DbSet<User_BTR> Users_BTR { get; set; }

        // ================== HOSE PLANT ==================
        public DbSet<Machine_HOSE> Machines_HOSE { get; set; }
        public DbSet<MachineNumber_HOSE> MachineNumbers_HOSE { get; set; }
        public DbSet<ChecklistItem_HOSE> ChecklistItems_HOSE { get; set; }
        public DbSet<InspectionSession_HOSE> InspectionSessions_HOSE { get; set; }
        public DbSet<InspectionResult_HOSE> InspectionResults_HOSE { get; set; }
        public DbSet<User_HOSE> Users_HOSE { get; set; }

        // ================== MOLDED PLANT ==================
        public DbSet<Machine_MOLDED> Machines_MOLDED { get; set; }
        public DbSet<MachineNumber_MOLDED> MachineNumbers_MOLDED { get; set; }
        public DbSet<ChecklistItem_MOLDED> ChecklistItems_MOLDED { get; set; }
        public DbSet<InspectionSession_MOLDED> InspectionSessions_MOLDED { get; set; }
        public DbSet<InspectionResult_MOLDED> InspectionResults_MOLDED { get; set; }
        public DbSet<User_MOLDED> Users_MOLDED { get; set; }

        // ================== MIXING PLANT ==================
        public DbSet<Machine_MIXING> Machines_MIXING { get; set; }
        public DbSet<MachineNumber_MIXING> MachineNumbers_MIXING { get; set; }
        public DbSet<ChecklistItem_MIXING> ChecklistItems_MIXING { get; set; }
        public DbSet<InspectionSession_MIXING> InspectionSessions_MIXING { get; set; }
        public DbSet<InspectionResult_MIXING> InspectionResults_MIXING { get; set; }
        public DbSet<User_MIXING> Users_MIXING { get; set; }

        // ================== ANDON SYSTEM ==================
        public DbSet<Plant> Plants { get; set; }
        public DbSet<AndonMachine> AndonMachines { get; set; }
        public DbSet<StatusType> StatusTypes { get; set; }
        public DbSet<FourMCategory> FourMCategories { get; set; }
        public DbSet<AndonRecord> AndonRecords { get; set; }
        public DbSet<ScwLog> ScwLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ================== RVI MAPPING ==================
            // Manual mapping for RVI classes (since they don't have [Table] attributes like the new ones)
            modelBuilder.Entity<Machine>().ToTable("Machines_RVI");
            modelBuilder.Entity<MachineNumber>().ToTable("MachineNumbers_RVI");
            modelBuilder.Entity<ChecklistItem>().ToTable("ChecklistItems_RVI");
            modelBuilder.Entity<InspectionSession>().ToTable("InspectionSessions_RVI");
            modelBuilder.Entity<InspectionResult>().ToTable("InspectionResults_RVI");
            modelBuilder.Entity<User>().ToTable("Users_RVI");

            // RVI Indexes & Constraints
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Machine>().HasIndex(m => m.Name);
            
            // Delete Behavior for RVI
            modelBuilder.Entity<InspectionResult>()
                .HasOne(ir => ir.InspectionSession)
                .WithMany(s => s.InspectionResults)
                .OnDelete(DeleteBehavior.Restrict); // Prevent Cycles
            modelBuilder.Entity<InspectionResult>()
                .HasOne(ir => ir.ChecklistItem)
                .WithMany(ci => ci.InspectionResults)
                .OnDelete(DeleteBehavior.Restrict); // Prevent Cycles


            // ================== NEW PLANTS INDEXES & CONSTRAINTS ==================
            // Since we use [Table] and [ForeignKey] annotations, we only need to add Indexes and Restrict Delete
            
            // BTR
            ConfigurePlant_BTR(modelBuilder);
            // HOSE
            ConfigurePlant_HOSE(modelBuilder);
            // MOLDED
            ConfigurePlant_MOLDED(modelBuilder);
            // MIXING
            ConfigurePlant_MIXING(modelBuilder);

            // Seed Data for RVI (Default)
            SeedDataRVI(modelBuilder);
            
            // Seed Data for Other Plants
            SeedData_BTR(modelBuilder);
            SeedData_HOSE(modelBuilder);
            SeedData_MOLDED(modelBuilder);
            SeedData_MIXING(modelBuilder);
            SeedShiftSettings(modelBuilder);

            // Configure and Seed Andon System
            ConfigureAndon(modelBuilder);
            SeedDataAndon(modelBuilder);

            // Mapping for Production SCW Logs (Cross-Database)
            modelBuilder.Entity<ScwLog>().ToTable("tb_elwp_produksi_scw_logs", t => t.ExcludeFromMigrations());
        }

        private void SeedShiftSettings(ModelBuilder modelBuilder)
        {
            var plants = new[] { "RVI", "BTR", "HOSE", "MOLDED", "MIXING" };
            int idCounter = 1;
            
            foreach (var plant in plants)
            {
                // Shift 1: 05:00 - 14:00 (8 jam kerja + 1 jam istirahat)
                modelBuilder.Entity<ShiftSetting>().HasData(
                    new ShiftSetting { Id = idCounter++, Plant = plant, ShiftNumber = 1, StartTime = new TimeSpan(5, 0, 0), EndTime = new TimeSpan(14, 0, 0) },
                    new ShiftSetting { Id = idCounter++, Plant = plant, ShiftNumber = 2, StartTime = new TimeSpan(14, 0, 0), EndTime = new TimeSpan(22, 0, 0) },
                    new ShiftSetting { Id = idCounter++, Plant = plant, ShiftNumber = 3, StartTime = new TimeSpan(22, 0, 0), EndTime = new TimeSpan(5, 0, 0) }
                );
            }
        }

        private void SeedData_BTR(ModelBuilder modelBuilder)
        {
             // Seed Admin User for BTR
            modelBuilder.Entity<User_BTR>().HasData(
                new User_BTR { Id = 1, Username = "admin", FullName = "Admin BTR", Email = "admin.btr@amrvi.com", Password = "admin123", Role = "Admin", Department = "IT", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );
        }

        private void SeedData_HOSE(ModelBuilder modelBuilder)
        {
             // Seed Admin User for HOSE
            modelBuilder.Entity<User_HOSE>().HasData(
                new User_HOSE { Id = 1, Username = "admin", FullName = "Admin HOSE", Email = "admin.hose@amrvi.com", Password = "admin123", Role = "Admin", Department = "IT", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );
        }

        private void SeedData_MOLDED(ModelBuilder modelBuilder)
        {
             // Seed Admin User for MOLDED
            modelBuilder.Entity<User_MOLDED>().HasData(
                new User_MOLDED { Id = 1, Username = "admin", FullName = "Admin MOLDED", Email = "admin.molded@amrvi.com", Password = "admin123", Role = "Admin", Department = "IT", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );
        }

        private void SeedData_MIXING(ModelBuilder modelBuilder)
        {
             // Seed Admin User for MIXING
            modelBuilder.Entity<User_MIXING>().HasData(
                new User_MIXING { Id = 1, Username = "admin", FullName = "Admin MIXING", Email = "admin.mixing@amrvi.com", Password = "admin123", Role = "Admin", Department = "IT", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );
        }

        private void ConfigurePlant_BTR(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_BTR>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<InspectionResult_BTR>()
                .HasOne(ir => ir.InspectionSession).WithMany(isess => isess.InspectionResults).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InspectionResult_BTR>()
                .HasOne(ir => ir.ChecklistItem).WithMany(c => c.InspectionResults).OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigurePlant_HOSE(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_HOSE>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<InspectionResult_HOSE>()
                .HasOne(ir => ir.InspectionSession).WithMany(isess => isess.InspectionResults).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InspectionResult_HOSE>()
                .HasOne(ir => ir.ChecklistItem).WithMany(c => c.InspectionResults).OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigurePlant_MOLDED(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_MOLDED>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<InspectionResult_MOLDED>()
                .HasOne(ir => ir.InspectionSession).WithMany(isess => isess.InspectionResults).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InspectionResult_MOLDED>()
                .HasOne(ir => ir.ChecklistItem).WithMany(c => c.InspectionResults).OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigurePlant_MIXING(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_MIXING>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<InspectionResult_MIXING>()
                .HasOne(ir => ir.InspectionSession).WithMany(isess => isess.InspectionResults).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<InspectionResult_MIXING>()
                .HasOne(ir => ir.ChecklistItem).WithMany(c => c.InspectionResults).OnDelete(DeleteBehavior.Restrict);
        }

        private void SeedDataRVI(ModelBuilder modelBuilder)
        {
            // Seed Machines (Using base class Machine which maps to Machines_RVI)
             modelBuilder.Entity<Machine>().HasData(
                new Machine { Id = 1, Name = "MESIN INJECTION", Description = "Injection Molding Machine", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Machine { Id = 2, Name = "MESIN PRESS", Description = "Press Machine", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Machine { Id = 3, Name = "MESIN CNC", Description = "CNC Machine", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );

            // Seed Machine Numbers
            modelBuilder.Entity<MachineNumber>().HasData(
                new MachineNumber { Id = 1, MachineId = 1, Number = "1", Location = "Area A", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new MachineNumber { Id = 2, MachineId = 1, Number = "2", Location = "Area A", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new MachineNumber { Id = 3, MachineId = 1, Number = "3", Location = "Area B", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new MachineNumber { Id = 4, MachineId = 2, Number = "1", Location = "Area C", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new MachineNumber { Id = 5, MachineId = 2, Number = "2", Location = "Area C", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new MachineNumber { Id = 6, MachineId = 3, Number = "1", Location = "Area D", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );

            // Seed Checklist Items
            modelBuilder.Entity<ChecklistItem>().HasData(
                new ChecklistItem { Id = 1, MachineId = 1, OrderNumber = 1, DetailName = "Emergency Stop", StandardDescription = "Pastikan tombol emergency stop berfungsi dengan baik dan mudah dijangkau", ImagePath = "/images/checklist/injection/emergency-stop.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 2, MachineId = 1, OrderNumber = 2, DetailName = "Safety Light Curtain", StandardDescription = "Pastikan pergerakan Ejector & Mold berhenti saat light curtain aktif", ImagePath = "/images/checklist/injection/safety-light-curtain.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 3, MachineId = 1, OrderNumber = 3, DetailName = "Safety Door", StandardDescription = "Pastikan pintu safety tertutup rapat dan interlock berfungsi", ImagePath = "/images/checklist/injection/safety-door.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 4, MachineId = 1, OrderNumber = 4, DetailName = "Hydraulic Pressure", StandardDescription = "Cek tekanan hydraulic dalam range normal (150-200 bar)", ImagePath = "/images/checklist/injection/hydraulic-pressure.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 5, MachineId = 1, OrderNumber = 5, DetailName = "Heating System", StandardDescription = "Pastikan suhu barrel sesuai dengan setting point", ImagePath = "/images/checklist/injection/heating-system.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 6, MachineId = 1, OrderNumber = 6, DetailName = "Cooling System", StandardDescription = "Cek aliran air cooling dan suhu", ImagePath = "/images/checklist/injection/cooling-system.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 7, MachineId = 1, OrderNumber = 7, DetailName = "Mold Condition", StandardDescription = "Pastikan mold bersih dan tidak ada kerusakan", ImagePath = "/images/checklist/injection/mold-condition.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 8, MachineId = 1, OrderNumber = 8, DetailName = "Ejector System", StandardDescription = "Cek pergerakan ejector smooth dan tidak macet", ImagePath = "/images/checklist/injection/ejector-system.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 9, MachineId = 1, OrderNumber = 9, DetailName = "Hopper Dryer", StandardDescription = "Pastikan material kering dan suhu dryer sesuai", ImagePath = "/images/checklist/injection/hopper-dryer.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 10, MachineId = 1, OrderNumber = 10, DetailName = "Robot Arm", StandardDescription = "Cek pergerakan robot arm dan vacuum system", ImagePath = "/images/checklist/injection/robot-arm.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 11, MachineId = 1, OrderNumber = 11, DetailName = "Oil Level", StandardDescription = "Pastikan level oli hydraulic dalam batas normal", ImagePath = "/images/checklist/injection/oil-level.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 12, MachineId = 1, OrderNumber = 12, DetailName = "Cleanliness", StandardDescription = "Area mesin bersih dari oli, material, dan kotoran", ImagePath = "/images/checklist/injection/cleanliness.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new ChecklistItem { Id = 13, MachineId = 1, OrderNumber = 13, DetailName = "Control Panel", StandardDescription = "Pastikan display dan tombol kontrol berfungsi normal", ImagePath = "/images/checklist/injection/control-panel.jpg", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );

            // Seed Users
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", FullName = "Administrator", Email = "admin@amrvi.com", Password = "admin123", Role = "Admin", Department = "IT", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new User { Id = 2, Username = "supervisor", FullName = "Supervisor Production", Email = "supervisor@amrvi.com", Password = "super123", Role = "Supervisor", Department = "Production", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new User { Id = 3, Username = "operator1", FullName = "Operator Mesin 1", Email = "operator1@amrvi.com", Password = "user123", Role = "User", Department = "Production", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );
        }

        private void ConfigureAndon(ModelBuilder modelBuilder)
        {
            // Configure unique constraints
            modelBuilder.Entity<Plant>()
                .HasIndex(p => p.PlantCode)
                .IsUnique();

            modelBuilder.Entity<AndonMachine>()
                .HasIndex(m => new { m.PlantId, m.MachineCode })
                .IsUnique();

            modelBuilder.Entity<StatusType>()
                .HasIndex(s => s.StatusCode)
                .IsUnique();

            modelBuilder.Entity<FourMCategory>()
                .HasIndex(f => f.CategoryCode)
                .IsUnique();

            // Configure indexes for performance
            modelBuilder.Entity<AndonRecord>()
                .HasIndex(a => a.PlantId);

            modelBuilder.Entity<AndonRecord>()
                .HasIndex(a => a.MachineId);

            modelBuilder.Entity<AndonRecord>()
                .HasIndex(a => a.RecordedAt);

            // Configure delete behavior
            modelBuilder.Entity<AndonRecord>()
                .HasOne(a => a.Plant)
                .WithMany(p => p.AndonRecords)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AndonRecord>()
                .HasOne(a => a.AndonMachine)
                .WithMany(m => m.AndonRecords)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AndonRecord>()
                .HasOne(a => a.StatusType)
                .WithMany(s => s.AndonRecords)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AndonRecord>()
                .HasOne(a => a.FourMCategory)
                .WithMany(f => f.AndonRecords)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void SeedDataAndon(ModelBuilder modelBuilder)
        {
            // Seed Plants
            modelBuilder.Entity<Plant>().HasData(
                new Plant { Id = 1, PlantCode = "RVI", PlantName = "Rubber Vibration Isolator", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Plant { Id = 2, PlantCode = "BTR", PlantName = "Bridgestone", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Plant { Id = 3, PlantCode = "HOSE", PlantName = "HOSE Production", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Plant { Id = 4, PlantCode = "MOLDED", PlantName = "MOLDED Production", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) },
                new Plant { Id = 5, PlantCode = "MIXING", PlantName = "MIXING Production", IsActive = true, CreatedAt = new DateTime(2025, 1, 1) }
            );

            // Seed StatusTypes
            modelBuilder.Entity<StatusType>().HasData(
                new StatusType { Id = 1, StatusCode = "LINE_STOP", StatusName = "LINE STOP", ColorCode = "#ef4444", Priority = 1 },
                new StatusType { Id = 2, StatusCode = "NO_LOADING", StatusName = "NO LOADING", ColorCode = "#3b82f6", Priority = 2 },
                new StatusType { Id = 3, StatusCode = "NO_RUNNING", StatusName = "NO RUNNING", ColorCode = "#f97316", Priority = 3 },
                new StatusType { Id = 4, StatusCode = "RUNNING", StatusName = "RUNNING", ColorCode = "#10b981", Priority = 4 }
            );

            // Seed FourMCategories
            modelBuilder.Entity<FourMCategory>().HasData(
                new FourMCategory { Id = 1, CategoryCode = "MACHINE", CategoryName = "MACHINE", ColorCode = "#ec4899", Priority = 1 },
                new FourMCategory { Id = 2, CategoryCode = "MATERIAL", CategoryName = "MATERIAL", ColorCode = "#eab308", Priority = 2 },
                new FourMCategory { Id = 3, CategoryCode = "MAN", CategoryName = "MAN", ColorCode = "#22d3ee", Priority = 3 },
                new FourMCategory { Id = 4, CategoryCode = "METHODE", CategoryName = "METHODE", ColorCode = "#a855f7", Priority = 4 },
                new FourMCategory { Id = 5, CategoryCode = "NO_PROBLEM", CategoryName = "NO PROBLEM", ColorCode = "#6b7280", Priority = 5 }
            );
        }
    }
}
