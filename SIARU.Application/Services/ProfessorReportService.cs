using SIARU.Application.DTOs.Reports;
using SIARU.Application.Interfaces;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class ProfessorReportService : IProfessorReportService
{
    private readonly IProfessorRepository _professorRepository;

    public ProfessorReportService(IProfessorRepository professorRepository)
    {
        _professorRepository = professorRepository;
    }

    public async Task<List<(int Id, string Name)>> SearchAsync(string term)
    {
        var professors = await _professorRepository.SearchByNameAsync(term);
        return professors
            .Select(p => (p.Id, p.Name))
            .ToList();
    }

    public async Task<ProfessorReportDto?> GetByIdAsync(int id)
    {
        var professor = await _professorRepository.GetReportByIdAsync(id);
        if (professor is null) return null;

        return new ProfessorReportDto
        {
            Id = professor.Id,
            Name = professor.Name,
            Office = professor.Office,
            KnowledgeArea = professor.KnowledgeArea.Name,
            Department = professor.KnowledgeArea.Department.Name,
            OfficeHours = professor.OfficeHours
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.DayOfWeek)
                .ThenBy(x => x.StartTime)
                .Select(x => new ProfessorOfficeHourDto
                {
                    DayOfWeek = x.DayOfWeek.ToString(),
                    StartTime = x.StartTime.ToString(@"HH\:mm"),
                    EndTime = x.EndTime.ToString(@"HH\:mm")
                })
                .ToList(),
            SubjectsTaught = professor.TeachingAssignments
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.AcademicYear)
                .ThenBy(x => x.SubjectCode)
                .Select(x => new ProfessorTeachingDto
                {
                    SubjectCode = x.SubjectCode,
                    SubjectName = x.Subject.Name,
                    AcademicYear = x.AcademicYear
                })
                .ToList()
        };
    }
}