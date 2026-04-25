namespace SIARU.Application.DTOs.Reports;

public class SubjectReportProfessorDto
{
    public int ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public string Office { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
}