using SIARU.Application.DTOs.Reports;

namespace SIARU.Application.Interfaces;

public interface IProfessorReportService
{
    Task<List<ProfessorReportSearchResultDto>> SearchAsync(string? term);
    Task<ProfessorReportDto?> GetByIdAsync(int id);
}