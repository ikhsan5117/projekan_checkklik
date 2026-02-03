using System.ComponentModel.DataAnnotations;

namespace AMRVI.Models.Interfaces
{
    public interface IMachine
    {
        int Id { get; set; }
        string Name { get; set; }
        string? Description { get; set; }
        bool IsActive { get; set; }
        DateTime CreatedAt { get; set; }
    }

    public interface IMachineNumber
    {
        int Id { get; set; }
        int MachineId { get; set; }
        string Number { get; set; }
        string? Location { get; set; }
        bool IsActive { get; set; }
        DateTime CreatedAt { get; set; }
    }

    public interface IChecklistItem
    {
        int Id { get; set; }
        int MachineId { get; set; }
        int OrderNumber { get; set; }
        string DetailName { get; set; }
        string StandardDescription { get; set; }
        string? ImagePath { get; set; }
        bool IsActive { get; set; }
        DateTime CreatedAt { get; set; }
    }

    public interface IInspectionSession
    {
        int Id { get; set; }
        int MachineNumberId { get; set; }
        string InspectorName { get; set; }
        DateTime InspectionDate { get; set; }
        bool IsCompleted { get; set; }
        DateTime? CompletedAt { get; set; }
        DateTime CreatedAt { get; set; }
    }

    public interface IInspectionResult
    {
        int Id { get; set; }
        int InspectionSessionId { get; set; }
        int ChecklistItemId { get; set; }
        string Judgement { get; set; }
        string? Remarks { get; set; }
        DateTime CreatedAt { get; set; }
    }

    public interface IUser
    {
        int Id { get; set; }
        string Username { get; set; }
        string FullName { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Role { get; set; }
        string? Department { get; set; }
        bool IsActive { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? LastLogin { get; set; }
    }
}
