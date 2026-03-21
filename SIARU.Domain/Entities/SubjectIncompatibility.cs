namespace SIARU.Domain.Entities;

public class SubjectIncompatibility
{
    public string SubjectCode { get; set; } = null!;
    public Subject Subject { get; set; } = null!;

    public string IncompatibleWithSubjectCode { get; set; } = null!;
    public Subject IncompatibleWithSubject { get; set; } = null!;
}