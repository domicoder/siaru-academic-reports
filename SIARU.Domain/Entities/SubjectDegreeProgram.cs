namespace SIARU.Domain.Entities;

public class SubjectDegreeProgram
{
    public string SubjectCode { get; set; } = null!;
    public Subject Subject { get; set; } = null!;

    public int DegreeProgramId { get; set; }
    public DegreeProgram DegreeProgram { get; set; } = null!;

    public bool IsFreeConfiguration { get; set; }

    public int Quadrimester { get; set; }
}