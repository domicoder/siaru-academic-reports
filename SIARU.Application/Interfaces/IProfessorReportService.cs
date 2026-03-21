using SIARU.Application.DTOs.Reports;

namespace SIARU.Application.Interfaces;

public interface IProfessorReportService
{
    Task<List<(int Id, string Name)>> SearchAsync(string term);
    Task<ProfessorReportDto?> GetByIdAsync(int id);
}