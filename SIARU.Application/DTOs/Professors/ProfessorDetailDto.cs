namespace SIARU.Application.DTOs.Professors;

public class ProfessorDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Office { get; set; } = string.Empty;
    public int KnowledgeAreaId { get; set; }
    public string KnowledgeAreaName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
}