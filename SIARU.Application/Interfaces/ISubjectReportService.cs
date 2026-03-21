using SIARU.Application.DTOs.Reports;

namespace SIARU.Application.Interfaces;

public interface ISubjectReportService
{
    Task<List<(string Code, string Name)>> SearchAsync(string term);
    Task<SubjectReportDto?> GetByCodeAsync(string code);
}