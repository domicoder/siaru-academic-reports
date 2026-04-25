using Microsoft.EntityFrameworkCore;
using SIARU.Application.DTOs.Reports;
using SIARU.Application.Interfaces;
using SIARU.Infrastructure.Persistence;

namespace SIARU.Infrastructure.Services;

public class ProfessorReportService : IProfessorReportService
{
    private readonly SiaruDbContext _context;

    public ProfessorReportService(SiaruDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProfessorReportSearchResultDto>> SearchAsync(string? term)
    {
        var query = _context.Professors
            .Include(x => x.KnowledgeArea)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            var normalized = term.Trim().ToLower();

            query = query.Where(x =>
                x.Name.ToLower().Contains(normalized) ||
                x.Office.ToLower().Contains(normalized));
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new ProfessorReportSearchResultDto
            {
                Id = x.Id,
                Name = x.Name,
                Office = x.Office,
                KnowledgeAreaName = x.KnowledgeArea.Name
            })
            .ToListAsync();
    }

    public async Task<ProfessorReportDto?> GetByIdAsync(int id)
    {
        var professor = await _context.Professors
            .Include(x => x.KnowledgeArea)
                .ThenInclude(x => x.Department)
            .Include(x => x.OfficeHours)
            .Include(x => x.TeachingAssignments)
                .ThenInclude(x => x.Subject)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (professor is null || professor.IsDeleted)
        {
            return null;
        }

        return new ProfessorReportDto
        {
            Id = professor.Id,
            Name = professor.Name,
            Office = professor.Office,
            KnowledgeAreaName = professor.KnowledgeArea.Name,
            DepartmentName = professor.KnowledgeArea.Department.Name,
            OfficeHours = professor.OfficeHours
                .OrderBy(x => x.DayOfWeek)
                .ThenBy(x => x.StartTime)
                .Select(x => new ProfessorReportOfficeHourDto
                {
                    DayOfWeek = x.DayOfWeek.ToString(),
                    StartTime = x.StartTime.ToString("HH\\:mm"),
                    EndTime = x.EndTime.ToString("HH\\:mm")
                })
                .ToList(),
            Subjects = professor.TeachingAssignments
                .OrderBy(x => x.AcademicYear)
                .ThenBy(x => x.Subject.Name)
                .Select(x => new ProfessorReportSubjectDto
                {
                    SubjectCode = x.SubjectCode,
                    SubjectName = x.Subject.Name,
                    AcademicYear = x.AcademicYear
                })
                .ToList()
        };
    }
}