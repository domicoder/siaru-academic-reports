namespace SIARU.Application.DTOs.TeachingAssignments;

public class TeachingAssignmentListDto
{
    public string SubjectCode { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public string KnowledgeAreaName { get; set; } = string.Empty;
    public string AcademicYear { get; set; } = string.Empty;
}