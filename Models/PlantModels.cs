using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AMRVI.Models.Interfaces;

namespace AMRVI.Models
{
    // ===========================
    // B R I G E S T O N E (BTR)
    // ===========================

    [Table("Machines_BTR")]
    public class Machine_BTR : IMachine
    {
        [Key] public int Id { get; set; }
        [Required][StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(500)] public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<MachineNumber_BTR> MachineNumbers { get; set; } = new List<MachineNumber_BTR>();
        public virtual ICollection<ChecklistItem_BTR> ChecklistItems { get; set; } = new List<ChecklistItem_BTR>();
    }

    [Table("MachineNumbers_BTR")]
    public class MachineNumber_BTR : IMachineNumber
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required][StringLength(50)] public string Number { get; set; } = string.Empty;
        [StringLength(200)] public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_BTR Machine { get; set; } = null!;
        public virtual ICollection<InspectionSession_BTR> InspectionSessions { get; set; } = new List<InspectionSession_BTR>();
    }

    [Table("ChecklistItems_BTR")]
    public class ChecklistItem_BTR : IChecklistItem
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required] public int OrderNumber { get; set; }
        [Required][StringLength(200)] public string DetailName { get; set; } = string.Empty;
        [Required][StringLength(1000)] public string StandardDescription { get; set; } = string.Empty;
        [StringLength(500)] public string? ImagePath { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_BTR Machine { get; set; } = null!;
        public virtual ICollection<InspectionResult_BTR> InspectionResults { get; set; } = new List<InspectionResult_BTR>();
    }

    [Table("InspectionSessions_BTR")]
    public class InspectionSession_BTR : IInspectionSession
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineNumberId { get; set; }
        [Required][StringLength(100)] public string InspectorName { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineNumberId")] public virtual MachineNumber_BTR MachineNumber { get; set; } = null!;
        public virtual ICollection<InspectionResult_BTR> InspectionResults { get; set; } = new List<InspectionResult_BTR>();
    }

    [Table("InspectionResults_BTR")]
    public class InspectionResult_BTR : IInspectionResult
    {
        [Key] public int Id { get; set; }
        [Required] public int InspectionSessionId { get; set; }
        [Required] public int ChecklistItemId { get; set; }
        [Required][StringLength(2)] public string Judgement { get; set; } = string.Empty; 
        [StringLength(500)] public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("InspectionSessionId")] public virtual InspectionSession_BTR InspectionSession { get; set; } = null!;
        [ForeignKey("ChecklistItemId")] public virtual ChecklistItem_BTR ChecklistItem { get; set; } = null!;
    }

    [Table("Users_BTR")]
    public class User_BTR : IUser
    {
        [Key] public int Id { get; set; }
        [Required][MaxLength(100)] public string Username { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Password { get; set; } = string.Empty; 
        [Required][MaxLength(50)] public string Role { get; set; } = "User";
        [MaxLength(50)] public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
    }

    // ===========================
    // H O S E
    // ===========================

    [Table("Machines_HOSE")]
    public class Machine_HOSE : IMachine
    {
        [Key] public int Id { get; set; }
        [Required][StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(500)] public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<MachineNumber_HOSE> MachineNumbers { get; set; } = new List<MachineNumber_HOSE>();
        public virtual ICollection<ChecklistItem_HOSE> ChecklistItems { get; set; } = new List<ChecklistItem_HOSE>();
    }

    [Table("MachineNumbers_HOSE")]
    public class MachineNumber_HOSE : IMachineNumber
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required][StringLength(50)] public string Number { get; set; } = string.Empty;
        [StringLength(200)] public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_HOSE Machine { get; set; } = null!;
        public virtual ICollection<InspectionSession_HOSE> InspectionSessions { get; set; } = new List<InspectionSession_HOSE>();
    }

    [Table("ChecklistItems_HOSE")]
    public class ChecklistItem_HOSE : IChecklistItem
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required] public int OrderNumber { get; set; }
        [Required][StringLength(200)] public string DetailName { get; set; } = string.Empty;
        [Required][StringLength(1000)] public string StandardDescription { get; set; } = string.Empty;
        [StringLength(500)] public string? ImagePath { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_HOSE Machine { get; set; } = null!;
        public virtual ICollection<InspectionResult_HOSE> InspectionResults { get; set; } = new List<InspectionResult_HOSE>();
    }

    [Table("InspectionSessions_HOSE")]
    public class InspectionSession_HOSE : IInspectionSession
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineNumberId { get; set; }
        [Required][StringLength(100)] public string InspectorName { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineNumberId")] public virtual MachineNumber_HOSE MachineNumber { get; set; } = null!;
        public virtual ICollection<InspectionResult_HOSE> InspectionResults { get; set; } = new List<InspectionResult_HOSE>();
    }

    [Table("InspectionResults_HOSE")]
    public class InspectionResult_HOSE : IInspectionResult
    {
        [Key] public int Id { get; set; }
        [Required] public int InspectionSessionId { get; set; }
        [Required] public int ChecklistItemId { get; set; }
        [Required][StringLength(2)] public string Judgement { get; set; } = string.Empty; 
        [StringLength(500)] public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("InspectionSessionId")] public virtual InspectionSession_HOSE InspectionSession { get; set; } = null!;
        [ForeignKey("ChecklistItemId")] public virtual ChecklistItem_HOSE ChecklistItem { get; set; } = null!;
    }

    [Table("Users_HOSE")]
    public class User_HOSE : IUser
    {
        [Key] public int Id { get; set; }
        [Required][MaxLength(100)] public string Username { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Password { get; set; } = string.Empty; 
        [Required][MaxLength(50)] public string Role { get; set; } = "User";
        [MaxLength(50)] public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
    }

     // ===========================
    // M O L D E D
    // ===========================

    [Table("Machines_MOLDED")]
    public class Machine_MOLDED : IMachine
    {
        [Key] public int Id { get; set; }
        [Required][StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(500)] public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<MachineNumber_MOLDED> MachineNumbers { get; set; } = new List<MachineNumber_MOLDED>();
        public virtual ICollection<ChecklistItem_MOLDED> ChecklistItems { get; set; } = new List<ChecklistItem_MOLDED>();
    }

    [Table("MachineNumbers_MOLDED")]
    public class MachineNumber_MOLDED : IMachineNumber
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required][StringLength(50)] public string Number { get; set; } = string.Empty;
        [StringLength(200)] public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_MOLDED Machine { get; set; } = null!;
        public virtual ICollection<InspectionSession_MOLDED> InspectionSessions { get; set; } = new List<InspectionSession_MOLDED>();
    }

    [Table("ChecklistItems_MOLDED")]
    public class ChecklistItem_MOLDED : IChecklistItem
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required] public int OrderNumber { get; set; }
        [Required][StringLength(200)] public string DetailName { get; set; } = string.Empty;
        [Required][StringLength(1000)] public string StandardDescription { get; set; } = string.Empty;
        [StringLength(500)] public string? ImagePath { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_MOLDED Machine { get; set; } = null!;
        public virtual ICollection<InspectionResult_MOLDED> InspectionResults { get; set; } = new List<InspectionResult_MOLDED>();
    }

    [Table("InspectionSessions_MOLDED")]
    public class InspectionSession_MOLDED : IInspectionSession
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineNumberId { get; set; }
        [Required][StringLength(100)] public string InspectorName { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineNumberId")] public virtual MachineNumber_MOLDED MachineNumber { get; set; } = null!;
        public virtual ICollection<InspectionResult_MOLDED> InspectionResults { get; set; } = new List<InspectionResult_MOLDED>();
    }

    [Table("InspectionResults_MOLDED")]
    public class InspectionResult_MOLDED : IInspectionResult
    {
        [Key] public int Id { get; set; }
        [Required] public int InspectionSessionId { get; set; }
        [Required] public int ChecklistItemId { get; set; }
        [Required][StringLength(2)] public string Judgement { get; set; } = string.Empty; 
        [StringLength(500)] public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("InspectionSessionId")] public virtual InspectionSession_MOLDED InspectionSession { get; set; } = null!;
        [ForeignKey("ChecklistItemId")] public virtual ChecklistItem_MOLDED ChecklistItem { get; set; } = null!;
    }

    [Table("Users_MOLDED")]
    public class User_MOLDED : IUser
    {
        [Key] public int Id { get; set; }
        [Required][MaxLength(100)] public string Username { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Password { get; set; } = string.Empty; 
        [Required][MaxLength(50)] public string Role { get; set; } = "User";
        [MaxLength(50)] public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
    }

     // ===========================
    // M I X I N G
    // ===========================

    [Table("Machines_MIXING")]
    public class Machine_MIXING : IMachine
    {
        [Key] public int Id { get; set; }
        [Required][StringLength(100)] public string Name { get; set; } = string.Empty;
        [StringLength(500)] public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<MachineNumber_MIXING> MachineNumbers { get; set; } = new List<MachineNumber_MIXING>();
        public virtual ICollection<ChecklistItem_MIXING> ChecklistItems { get; set; } = new List<ChecklistItem_MIXING>();
    }

    [Table("MachineNumbers_MIXING")]
    public class MachineNumber_MIXING : IMachineNumber
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required][StringLength(50)] public string Number { get; set; } = string.Empty;
        [StringLength(200)] public string? Location { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_MIXING Machine { get; set; } = null!;
        public virtual ICollection<InspectionSession_MIXING> InspectionSessions { get; set; } = new List<InspectionSession_MIXING>();
    }

    [Table("ChecklistItems_MIXING")]
    public class ChecklistItem_MIXING : IChecklistItem
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineId { get; set; }
        [Required] public int OrderNumber { get; set; }
        [Required][StringLength(200)] public string DetailName { get; set; } = string.Empty;
        [Required][StringLength(1000)] public string StandardDescription { get; set; } = string.Empty;
        [StringLength(500)] public string? ImagePath { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineId")] public virtual Machine_MIXING Machine { get; set; } = null!;
        public virtual ICollection<InspectionResult_MIXING> InspectionResults { get; set; } = new List<InspectionResult_MIXING>();
    }

    [Table("InspectionSessions_MIXING")]
    public class InspectionSession_MIXING : IInspectionSession
    {
        [Key] public int Id { get; set; }
        [Required] public int MachineNumberId { get; set; }
        [Required][StringLength(100)] public string InspectorName { get; set; } = string.Empty;
        public DateTime InspectionDate { get; set; } = DateTime.Now;
        public bool IsCompleted { get; set; } = false;
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("MachineNumberId")] public virtual MachineNumber_MIXING MachineNumber { get; set; } = null!;
        public virtual ICollection<InspectionResult_MIXING> InspectionResults { get; set; } = new List<InspectionResult_MIXING>();
    }

    [Table("InspectionResults_MIXING")]
    public class InspectionResult_MIXING : IInspectionResult
    {
        [Key] public int Id { get; set; }
        [Required] public int InspectionSessionId { get; set; }
        [Required] public int ChecklistItemId { get; set; }
        [Required][StringLength(2)] public string Judgement { get; set; } = string.Empty; 
        [StringLength(500)] public string? Remarks { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("InspectionSessionId")] public virtual InspectionSession_MIXING InspectionSession { get; set; } = null!;
        [ForeignKey("ChecklistItemId")] public virtual ChecklistItem_MIXING ChecklistItem { get; set; } = null!;
    }

    [Table("Users_MIXING")]
    public class User_MIXING : IUser
    {
        [Key] public int Id { get; set; }
        [Required][MaxLength(100)] public string Username { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string FullName { get; set; } = string.Empty;
        [Required][EmailAddress][MaxLength(150)] public string Email { get; set; } = string.Empty;
        [Required][MaxLength(100)] public string Password { get; set; } = string.Empty; 
        [Required][MaxLength(50)] public string Role { get; set; } = "User";
        [MaxLength(50)] public string? Department { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLogin { get; set; }
    }
}
