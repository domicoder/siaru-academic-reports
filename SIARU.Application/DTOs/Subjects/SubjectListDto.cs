namespace SIARU.Application.DTOs.Subjects;

public class SubjectListDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Course { get; set; }
    public string KnowledgeAreaName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}