using SIARU.Domain.Common;
using SIARU.Domain.Enums;

namespace SIARU.Domain.Entities;

public class Subject : AuditableSoftDeletableEntity
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Course { get; set; }
    public decimal TheoreticalCredits { get; set; }
    public decimal LabCredits { get; set; }
    public SubjectType Type { get; set; }
    public int? AdmissionLimit { get; set; }

    public int KnowledgeAreaId { get; set; }
    public KnowledgeArea KnowledgeArea { get; set; } = null!;

    public ICollection<SubjectDegreeProgram> SubjectDegreePrograms { get; set; } = new List<SubjectDegreeProgram>();
    public ICollection<TeachingAssignment> TeachingAssignments { get; set; } = new List<TeachingAssignment>();

    public ICollection<SubjectIncompatibility> Incompatibilities { get; set; } = new List<SubjectIncompatibility>();
    public ICollection<SubjectIncompatibility> IncompatibleWithSubjects { get; set; } = new List<SubjectIncompatibility>();

    public ICollection<SubjectEquivalency> Equivalencies { get; set; } = new List<SubjectEquivalency>();
    public ICollection<SubjectEquivalency> EquivalentToSubjects { get; set; } = new List<SubjectEquivalency>();
}