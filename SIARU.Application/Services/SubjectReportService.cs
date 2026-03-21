using SIARU.Application.DTOs.Reports;
using SIARU.Application.Interfaces;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class SubjectReportService : ISubjectReportService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectReportService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<List<(string Code, string Name)>> SearchAsync(string term)
    {
        var subjects = await _subjectRepository.SearchByCodeOrNameAsync(term);
        return subjects
            .Select(s => (s.Code, s.Name))
            .ToList();
    }

    public async Task<SubjectReportDto?> GetByCodeAsync(string code)
    {
        var subject = await _subjectRepository.GetReportByCodeAsync(code);
        if (subject is null) return null;

        return new SubjectReportDto
        {
            Code = subject.Code,
            Name = subject.Name,
            Course = subject.Course,
            TheoreticalCredits = subject.TheoreticalCredits,
            LabCredits = subject.LabCredits,
            Type = subject.Type.ToString(),
            AdmissionLimit = subject.AdmissionLimit,
            KnowledgeArea = subject.KnowledgeArea.Name,
            Department = subject.KnowledgeArea.Department.Name,

            DegreePrograms = subject.SubjectDegreePrograms
                .OrderBy(x => x.Quadrimester)
                .ThenBy(x => x.DegreeProgram.Name)
                .Select(x => new SubjectDegreeProgramReportDto
                {
                    DegreeProgramName = x.DegreeProgram.Name,
                    Quadrimester = x.Quadrimester,
                    IsFreeConfiguration = x.IsFreeConfiguration
                })
                .ToList(),

            Professors = subject.TeachingAssignments
                .Where(x => !x.IsDeleted)
                .Select(x => x.Professor.Name)
                .Distinct()
                .OrderBy(x => x)
                .ToList(),

            Incompatibilities = subject.Incompatibilities
                .Select(x => $"{x.IncompatibleWithSubject.Code} - {x.IncompatibleWithSubject.Name}")
                .ToList(),

            Equivalencies = subject.Equivalencies
                .Select(x => $"{x.EquivalentToSubject.Code} - {x.EquivalentToSubject.Name}")
                .ToList()
        };
    }
}