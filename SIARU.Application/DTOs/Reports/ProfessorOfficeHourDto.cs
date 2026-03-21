namespace SIARU.Application.DTOs.Reports;

public class ProfessorOfficeHourDto
{
    public string DayOfWeek { get; set; } = null!;
    public string StartTime { get; set; } = null!;
    public string EndTime { get; set; } = null!;
}