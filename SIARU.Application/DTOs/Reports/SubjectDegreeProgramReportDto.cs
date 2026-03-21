namespace SIARU.Application.DTOs.Reports;

public class SubjectDegreeProgramReportDto
{
    public string DegreeProgramName { get; set; } = null!;
    public int Quadrimester { get; set; }
    public bool IsFreeConfiguration { get; set; }
}