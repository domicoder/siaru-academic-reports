namespace SIARU.Application.DTOs.Reports;

public class SubjectReportDegreeProgramDto
{
    public string DegreeProgramName { get; set; } = string.Empty;
    public int Quadrimester { get; set; }
    public bool IsFreeConfiguration { get; set; }
}