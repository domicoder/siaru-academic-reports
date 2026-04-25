using Microsoft.EntityFrameworkCore;
using SIARU.Application.DTOs.Reports;
using SIARU.Application.Interfaces;
using SIARU.Infrastructure.Persistence;

namespace SIARU.Infrastructure.Services;

public class SubjectReportService : ISubjectReportService
{
    private readonly SiaruDbContext _context;

    public SubjectReportService(SiaruDbContext context)
    {
        _context = context;
    }

    public async Task<List<SubjectReportSearchResultDto>> SearchAsync(string? term)
    {
        var query = _context.Subjects
            .Include(x => x.KnowledgeArea)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(term))
        {
            var normalized = term.Trim().ToLower();

            query = query.Where(x =>
                x.Code.ToLower().Contains(normalized) ||
                x.Name.ToLower().Contains(normalized));
        }

        return await query
            .OrderBy(x => x.Name)
            .Select(x => new SubjectReportSearchResultDto
            {
                Code = x.Code,
                Name = x.Name,
                KnowledgeAreaName = x.KnowledgeArea.Name
            })
            .ToListAsync();
    }

    public async Task<SubjectReportDto?> GetByCodeAsync(string code)
    {
        var subject = await _context.Subjects
            .Include(x => x.KnowledgeArea)
                .ThenInclude(x => x.Department)
            .Include(x => x.SubjectDegreePrograms)
                .ThenInclude(x => x.DegreeProgram)
            .Include(x => x.TeachingAssignments)
                .ThenInclude(x => x.Professor)
            .FirstOrDefaultAsync(x => x.Code == code);

        if (subject is null || subject.IsDeleted)
        {
            return null;
        }

        return new SubjectReportDto
        {
            Code = subject.Code,
            Name = subject.Name,
            Course = subject.Course,
            Type = subject.Type.ToString(),
            TheoreticalCredits = subject.TheoreticalCredits,
            LabCredits = subject.LabCredits,
            AdmissionLimit = subject.AdmissionLimit,
            KnowledgeAreaName = subject.KnowledgeArea.Name,
            DepartmentName = subject.KnowledgeArea.Department.Name,
            DegreePrograms = subject.SubjectDegreePrograms
                .OrderBy(x => x.Quadrimester)
                .ThenBy(x => x.DegreeProgram.Name)
                .Select(x => new SubjectReportDegreeProgramDto
                {
                    DegreeProgramName = x.DegreeProgram.Name,
                    Quadrimester = x.Quadrimester,
                    IsFreeConfiguration = x.IsFreeConfiguration
                })
                .ToList(),
            Professors = subject.TeachingAssignments
                .OrderBy(x => x.AcademicYear)
                .ThenBy(x => x.Professor.Name)
                .Select(x => new SubjectReportProfessorDto
                {
                    ProfessorId = x.ProfessorId,
                    ProfessorName = x.Professor.Name,
                    Office = x.Professor.Office,
                    AcademicYear = x.AcademicYear
                })
                .ToList()
        };
    }
}