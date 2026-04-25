namespace SIARU.Application.DTOs.Subjects;

public class SubjectDetailDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Course { get; set; }
    public decimal TheoreticalCredits { get; set; }
    public decimal LabCredits { get; set; }
    public string Type { get; set; } = string.Empty;
    public int? AdmissionLimit { get; set; }
    public int KnowledgeAreaId { get; set; }
    public string KnowledgeAreaName { get; set; } = string.Empty;

    public List<SubjectDegreeProgramAssignmentDto> DegreePrograms { get; set; } = new();
}