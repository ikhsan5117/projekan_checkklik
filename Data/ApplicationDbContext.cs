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

        public DbSet<Machine> Machines { get; set; }
        public DbSet<MachineNumber> MachineNumbers { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        public DbSet<InspectionSession> InspectionSessions { get; set; }
        public DbSet<InspectionResult> InspectionResults { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes
            modelBuilder.Entity<Machine>()
                .HasIndex(m => m.Name);

            modelBuilder.Entity<MachineNumber>()
                .HasIndex(mn => new { mn.MachineId, mn.Number });

            modelBuilder.Entity<ChecklistItem>()
                .HasIndex(ci => new { ci.MachineId, ci.OrderNumber });

            modelBuilder.Entity<InspectionSession>()
                .HasIndex(i => i.InspectionDate);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Configure DeleteBehavior to prevent cycles or multiple cascade paths
            modelBuilder.Entity<InspectionResult>()
                .HasOne(ir => ir.InspectionSession)
                .WithMany(s => s.InspectionResults)
                .HasForeignKey(ir => ir.InspectionSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InspectionResult>()
                .HasOne(ir => ir.ChecklistItem)
                .WithMany(ci => ci.InspectionResults)
                .HasForeignKey(ir => ir.ChecklistItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Machines
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

            // Seed Checklist Items for MESIN INJECTION (13 items based on your screenshot)
            modelBuilder.Entity<ChecklistItem>().HasData(
                new ChecklistItem 
                { 
                    Id = 1, 
                    MachineId = 1, 
                    OrderNumber = 1, 
                    DetailName = "Emergency Stop", 
                    StandardDescription = "Pastikan tombol emergency stop berfungsi dengan baik dan mudah dijangkau",
                    ImagePath = "/images/checklist/injection/emergency-stop.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 2, 
                    MachineId = 1, 
                    OrderNumber = 2, 
                    DetailName = "Safety Light Curtain", 
                    StandardDescription = "Pastikan pergerakan Ejector & Mold berhenti saat light curtain aktif",
                    ImagePath = "/images/checklist/injection/safety-light-curtain.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 3, 
                    MachineId = 1, 
                    OrderNumber = 3, 
                    DetailName = "Safety Door", 
                    StandardDescription = "Pastikan pintu safety tertutup rapat dan interlock berfungsi",
                    ImagePath = "/images/checklist/injection/safety-door.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 4, 
                    MachineId = 1, 
                    OrderNumber = 4, 
                    DetailName = "Hydraulic Pressure", 
                    StandardDescription = "Cek tekanan hydraulic dalam range normal (150-200 bar)",
                    ImagePath = "/images/checklist/injection/hydraulic-pressure.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 5, 
                    MachineId = 1, 
                    OrderNumber = 5, 
                    DetailName = "Heating System", 
                    StandardDescription = "Pastikan suhu barrel sesuai dengan setting point",
                    ImagePath = "/images/checklist/injection/heating-system.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 6, 
                    MachineId = 1, 
                    OrderNumber = 6, 
                    DetailName = "Cooling System", 
                    StandardDescription = "Cek aliran air cooling dan suhu",
                    ImagePath = "/images/checklist/injection/cooling-system.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 7, 
                    MachineId = 1, 
                    OrderNumber = 7, 
                    DetailName = "Mold Condition", 
                    StandardDescription = "Pastikan mold bersih dan tidak ada kerusakan",
                    ImagePath = "/images/checklist/injection/mold-condition.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 8, 
                    MachineId = 1, 
                    OrderNumber = 8, 
                    DetailName = "Ejector System", 
                    StandardDescription = "Cek pergerakan ejector smooth dan tidak macet",
                    ImagePath = "/images/checklist/injection/ejector-system.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 9, 
                    MachineId = 1, 
                    OrderNumber = 9, 
                    DetailName = "Hopper Dryer", 
                    StandardDescription = "Pastikan material kering dan suhu dryer sesuai",
                    ImagePath = "/images/checklist/injection/hopper-dryer.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 10, 
                    MachineId = 1, 
                    OrderNumber = 10, 
                    DetailName = "Robot Arm", 
                    StandardDescription = "Cek pergerakan robot arm dan vacuum system",
                    ImagePath = "/images/checklist/injection/robot-arm.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 11, 
                    MachineId = 1, 
                    OrderNumber = 11, 
                    DetailName = "Oil Level", 
                    StandardDescription = "Pastikan level oli hydraulic dalam batas normal",
                    ImagePath = "/images/checklist/injection/oil-level.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 12, 
                    MachineId = 1, 
                    OrderNumber = 12, 
                    DetailName = "Cleanliness", 
                    StandardDescription = "Area mesin bersih dari oli, material, dan kotoran",
                    ImagePath = "/images/checklist/injection/cleanliness.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                },
                new ChecklistItem 
                { 
                    Id = 13, 
                    MachineId = 1, 
                    OrderNumber = 13, 
                    DetailName = "Control Panel", 
                    StandardDescription = "Pastikan display dan tombol kontrol berfungsi normal",
                    ImagePath = "/images/checklist/injection/control-panel.jpg",
                    IsActive = true, 
                    CreatedAt = new DateTime(2025, 1, 1) 
                }
            );

            // Seed Users (password is hashed "password123" for demo)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    FullName = "Administrator",
                    Email = "admin@amrvi.com",
                    Password = "admin123", // Plain text (DEV ONLY)
                    Role = "Admin",
                    Department = "IT",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new User
                {
                    Id = 2,
                    Username = "supervisor",
                    FullName = "Supervisor Production",
                    Email = "supervisor@amrvi.com",
                    Password = "super123", // Plain text (DEV ONLY)
                    Role = "Supervisor",
                    Department = "Production",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1)
                },
                new User
                {
                    Id = 3,
                    Username = "operator1",
                    FullName = "Operator Mesin 1",
                    Email = "operator1@amrvi.com",
                    Password = "user123", // Plain text (DEV ONLY)
                    Role = "User",
                    Department = "Production",
                    IsActive = true,
                    CreatedAt = new DateTime(2025, 1, 1)
                }
            );
        }
    }
}
