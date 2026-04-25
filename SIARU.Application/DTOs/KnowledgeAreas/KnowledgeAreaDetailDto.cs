namespace SIARU.Application.DTOs.KnowledgeAreas;

public class KnowledgeAreaDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
}