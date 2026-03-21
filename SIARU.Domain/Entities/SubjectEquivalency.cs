namespace SIARU.Domain.Entities;

public class SubjectEquivalency
{
    public string SubjectCode { get; set; } = null!;
    public Subject Subject { get; set; } = null!;

    public string EquivalentToSubjectCode { get; set; } = null!;
    public Subject EquivalentToSubject { get; set; } = null!;
}