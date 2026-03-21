using SIARU.Domain.Common;

namespace SIARU.Domain.Entities;

public class Professor : AuditableSoftDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Office { get; set; }

    public int KnowledgeAreaId { get; set; }
    public KnowledgeArea KnowledgeArea { get; set; } = null!;

    public ICollection<OfficeHour> OfficeHours { get; set; } = new List<OfficeHour>();
    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();
}