namespace SIARU.Application.DTOs.Reports;

public class SubjectReportDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Course { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal TheoreticalCredits { get; set; }
    public decimal LabCredits { get; set; }
    public int? AdmissionLimit { get; set; }

    public string KnowledgeAreaName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;

    public List<SubjectReportDegreeProgramDto> DegreePrograms { get; set; } = new();
    public List<SubjectReportProfessorDto> Professors { get; set; } = new();
}