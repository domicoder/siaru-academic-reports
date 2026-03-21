using SIARU.Domain.Common;

namespace SIARU.Domain.Entities;

public class TeachingAssignment : AuditableSoftDeletableEntity
{
    public string SubjectCode { get; set; } = null!;
    public Subject Subject { get; set; } = null!;

    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;

    public string AcademicYear { get; set; } = null!;
}