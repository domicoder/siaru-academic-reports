using SIARU.Application.DTOs.Reports;

namespace SIARU.Application.Interfaces;

public interface ISubjectReportService
{
    Task<List<SubjectReportSearchResultDto>> SearchAsync(string? term);
    Task<SubjectReportDto?> GetByCodeAsync(string code);
}