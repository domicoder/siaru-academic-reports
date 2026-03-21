using SIARU.Domain.Common;

namespace SIARU.Domain.Entities;

public class DegreeProgram : AuditableSoftDeletableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<SubjectDegreeProgram> SubjectDegreePrograms { get; set; } = new List<SubjectDegreeProgram>();
}