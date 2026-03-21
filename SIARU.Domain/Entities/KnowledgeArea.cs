using SIARU.Domain.Common;

namespace SIARU.Domain.Entities;

public class KnowledgeArea : AuditableSoftDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<Professor> Professors { get; set; } = new List<Professor>();
}