namespace SIARU.Application.DTOs.OfficeHours;

public class OfficeHourListDto
{
    public int Id { get; set; }
    public int ProfessorId { get; set; }
    public string ProfessorName { get; set; } = string.Empty;
    public string ProfessorOffice { get; set; } = string.Empty;
    public string KnowledgeAreaName { get; set; } = string.Empty;
    public string DayOfWeek { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
}