namespace SIARU.Application.DTOs.Reports;

public class ProfessorReportDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Office { get; set; }

    public string KnowledgeArea { get; set; } = null!;
    public string Department { get; set; } = null!;

    public List<ProfessorOfficeHourDto> OfficeHours { get; set; } = new();
    public List<ProfessorTeachingDto> SubjectsTaught { get; set; } = new();
}