namespace SIARU.Application.DTOs.Reports;

public class SubjectReportDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public int Course { get; set; }
    public decimal TheoreticalCredits { get; set; }
    public decimal LabCredits { get; set; }
    public string Type { get; set; } = null!;
    public int? AdmissionLimit { get; set; }

    public string KnowledgeArea { get; set; } = null!;
    public string Department { get; set; } = null!;

    public List<SubjectDegreeProgramReportDto> DegreePrograms { get; set; } = new();
    public List<string> Professors { get; set; } = new();
    public List<string> Incompatibilities { get; set; } = new();
    public List<string> Equivalencies { get; set; } = new();
}