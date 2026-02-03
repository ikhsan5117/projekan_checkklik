using AMRVI.Data;
using AMRVI.Models;
using AMRVI.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMRVI.Services
{
    public class PlantService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PlantService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentPlant()
        {
            var plant = _httpContextAccessor.HttpContext?.User.FindFirst("Plant")?.Value;
            return plant ?? "RVI"; // Default ke RVI kalau belum login atau belum pilih
        }

        public IQueryable<IMachine> GetMachines()
        {
            var plant = GetCurrentPlant();
            return plant switch
            {
                "BTR" => _context.Machines_BTR.AsQueryable<IMachine>(),
                "HOSE" => _context.Machines_HOSE.AsQueryable<IMachine>(),
                "MOLDED" => _context.Machines_MOLDED.AsQueryable<IMachine>(),
                "MIXING" => _context.Machines_MIXING.AsQueryable<IMachine>(),
                _ => _context.Machines.AsQueryable<IMachine>() // RVI default
            };
        }

        public IQueryable<IMachineNumber> GetMachineNumbers()
        {
            var plant = GetCurrentPlant();
            return plant switch
            {
                "BTR" => _context.MachineNumbers_BTR.AsQueryable<IMachineNumber>(),
                "HOSE" => _context.MachineNumbers_HOSE.AsQueryable<IMachineNumber>(),
                "MOLDED" => _context.MachineNumbers_MOLDED.AsQueryable<IMachineNumber>(),
                "MIXING" => _context.MachineNumbers_MIXING.AsQueryable<IMachineNumber>(),
                _ => _context.MachineNumbers.AsQueryable<IMachineNumber>()
            };
        }

        public IQueryable<IChecklistItem> GetChecklistItems()
        {
            var plant = GetCurrentPlant();
            return plant switch
            {
                "BTR" => _context.ChecklistItems_BTR.AsQueryable<IChecklistItem>(),
                "HOSE" => _context.ChecklistItems_HOSE.AsQueryable<IChecklistItem>(),
                "MOLDED" => _context.ChecklistItems_MOLDED.AsQueryable<IChecklistItem>(),
                "MIXING" => _context.ChecklistItems_MIXING.AsQueryable<IChecklistItem>(),
                _ => _context.ChecklistItems.AsQueryable<IChecklistItem>()
            };
        }

        public IQueryable<IInspectionSession> GetInspectionSessions()
        {
            var plant = GetCurrentPlant();
            return plant switch
            {
                "BTR" => _context.InspectionSessions_BTR.AsQueryable<IInspectionSession>(),
                "HOSE" => _context.InspectionSessions_HOSE.AsQueryable<IInspectionSession>(),
                "MOLDED" => _context.InspectionSessions_MOLDED.AsQueryable<IInspectionSession>(),
                "MIXING" => _context.InspectionSessions_MIXING.AsQueryable<IInspectionSession>(),
                _ => _context.InspectionSessions.AsQueryable<IInspectionSession>()
            };
        }

        public IQueryable<IInspectionResult> GetInspectionResults()
        {
            var plant = GetCurrentPlant();
            return plant switch
            {
                "BTR" => _context.InspectionResults_BTR.AsQueryable<IInspectionResult>(),
                "HOSE" => _context.InspectionResults_HOSE.AsQueryable<IInspectionResult>(),
                "MOLDED" => _context.InspectionResults_MOLDED.AsQueryable<IInspectionResult>(),
                "MIXING" => _context.InspectionResults_MIXING.AsQueryable<IInspectionResult>(),
            _ => _context.InspectionResults.AsQueryable<IInspectionResult>()
            };
        }

        public IQueryable<IUser> GetUsers()
        {
            var plant = GetCurrentPlant();
            return plant switch
            {
                "BTR" => _context.Users_BTR.AsQueryable<IUser>(),
                "HOSE" => _context.Users_HOSE.AsQueryable<IUser>(),
                "MOLDED" => _context.Users_MOLDED.AsQueryable<IUser>(),
                "MIXING" => _context.Users_MIXING.AsQueryable<IUser>(),
                _ => _context.Users.AsQueryable<IUser>()
            };
        }

        public string GetPlantName()
        {
            return GetCurrentPlant();
        }
    }
}
