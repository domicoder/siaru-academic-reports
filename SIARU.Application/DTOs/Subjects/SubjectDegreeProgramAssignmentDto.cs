namespace SIARU.Application.DTOs.Subjects;

public class SubjectDegreeProgramAssignmentDto
{
    public int DegreeProgramId { get; set; }
    public int Quadrimester { get; set; }
    public bool IsFreeConfiguration { get; set; }
}