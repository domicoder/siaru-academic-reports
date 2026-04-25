using SIARU.Application.DTOs.Subjects;
using SIARU.Application.Interfaces;
using SIARU.Domain.Entities;
using SIARU.Domain.Enums;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly ISubjectManagementRepository _subjectManagementRepository;

    public SubjectService(ISubjectManagementRepository subjectManagementRepository)
    {
        _subjectManagementRepository = subjectManagementRepository;
    }

    public async Task<List<SubjectListDto>> GetAllAsync()
    {
        var items = await _subjectManagementRepository.GetAllWithKnowledgeAreaAsync();

        return items
            .Select(x => new SubjectListDto
            {
                Code = x.Code,
                Name = x.Name,
                Course = x.Course,
                KnowledgeAreaName = x.KnowledgeArea.Name,
                Type = x.Type.ToString()
            })
            .ToList();
    }

    public async Task<SubjectDetailDto?> GetByCodeAsync(string code)
    {
        var item = await _subjectManagementRepository.GetByCodeWithDetailsAsync(code);

        if (item is null || item.IsDeleted)
        {
            return null;
        }

        return new SubjectDetailDto
        {
            Code = item.Code,
            Name = item.Name,
            Course = item.Course,
            TheoreticalCredits = item.TheoreticalCredits,
            LabCredits = item.LabCredits,
            Type = item.Type.ToString(),
            AdmissionLimit = item.AdmissionLimit,
            KnowledgeAreaId = item.KnowledgeAreaId,
            KnowledgeAreaName = item.KnowledgeArea.Name,
            DegreePrograms = item.SubjectDegreePrograms
                .OrderBy(x => x.Quadrimester)
                .ThenBy(x => x.DegreeProgram.Name)
                .Select(x => new SubjectDegreeProgramAssignmentDto
                {
                    DegreeProgramId = x.DegreeProgramId,
                    Quadrimester = x.Quadrimester,
                    IsFreeConfiguration = x.IsFreeConfiguration
                })
                .ToList()
        };
    }

    public async Task<string> CreateAsync(CreateSubjectDto dto)
    {
        await ValidateSubjectAsync(dto.Code, dto.KnowledgeAreaId, dto.DegreePrograms, isUpdate: false);

        var subject = new Subject
        {
            Code = dto.Code.Trim(),
            Name = dto.Name.Trim(),
            Course = dto.Course,
            TheoreticalCredits = dto.TheoreticalCredits,
            LabCredits = dto.LabCredits,
            Type = MapSubjectType(dto.Type),
            AdmissionLimit = dto.AdmissionLimit,
            KnowledgeAreaId = dto.KnowledgeAreaId
        };

        var assignments = dto.DegreePrograms
            .Select(x => new SubjectDegreeProgram
            {
                SubjectCode = subject.Code,
                DegreeProgramId = x.DegreeProgramId,
                Quadrimester = x.Quadrimester,
                IsFreeConfiguration = x.IsFreeConfiguration
            })
            .ToList();

        await _subjectManagementRepository.AddAsync(subject, assignments);
        await _subjectManagementRepository.SaveChangesAsync();

        return subject.Code;
    }

    public async Task<bool> UpdateAsync(UpdateSubjectDto dto)
    {
        var subject = await _subjectManagementRepository.GetByCodeWithDetailsAsync(dto.Code);

        if (subject is null || subject.IsDeleted)
        {
            return false;
        }

        await ValidateSubjectAsync(dto.Code, dto.KnowledgeAreaId, dto.DegreePrograms, isUpdate: true);

        subject.Name = dto.Name.Trim();
        subject.Course = dto.Course;
        subject.TheoreticalCredits = dto.TheoreticalCredits;
        subject.LabCredits = dto.LabCredits;
        subject.Type = MapSubjectType(dto.Type);
        subject.AdmissionLimit = dto.AdmissionLimit;
        subject.KnowledgeAreaId = dto.KnowledgeAreaId;

        var assignments = dto.DegreePrograms
            .Select(x => new SubjectDegreeProgram
            {
                SubjectCode = subject.Code,
                DegreeProgramId = x.DegreeProgramId,
                Quadrimester = x.Quadrimester,
                IsFreeConfiguration = x.IsFreeConfiguration
            })
            .ToList();

        await _subjectManagementRepository.UpdateAsync(subject, assignments);
        await _subjectManagementRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteAsync(string code)
    {
        var subject = await _subjectManagementRepository.GetByCodeWithDetailsAsync(code);

        if (subject is null || subject.IsDeleted)
        {
            return false;
        }

        subject.IsDeleted = true;
        subject.DeletedAt = DateTime.UtcNow;

        await _subjectManagementRepository.SoftDeleteAsync(subject);
        await _subjectManagementRepository.SaveChangesAsync();

        return true;
    }

    private async Task ValidateSubjectAsync(
        string code,
        int knowledgeAreaId,
        List<SubjectDegreeProgramAssignmentDto> degreePrograms,
        bool isUpdate)
    {
        var normalizedCode = code.Trim();

        if (!isUpdate)
        {
            var codeExists = await _subjectManagementRepository.CodeExistsAsync(normalizedCode);
            if (codeExists)
            {
                throw new InvalidOperationException("Ya existe una asignatura con ese código.");
            }
        }

        var knowledgeAreaExists = await _subjectManagementRepository.KnowledgeAreaExistsAsync(knowledgeAreaId);
        if (!knowledgeAreaExists)
        {
            throw new InvalidOperationException("El área de conocimiento seleccionada no existe.");
        }

        if (degreePrograms is null || degreePrograms.Count == 0)
        {
            throw new InvalidOperationException("Debes registrar al menos una titulación para la asignatura.");
        }

        var duplicateDegreePrograms = degreePrograms
            .GroupBy(x => x.DegreeProgramId)
            .Any(g => g.Count() > 1);

        if (duplicateDegreePrograms)
        {
            throw new InvalidOperationException("No puedes repetir la misma titulación dentro de una asignatura.");
        }

        foreach (var assignment in degreePrograms)
        {
            if (assignment.Quadrimester < 1 || assignment.Quadrimester > 20)
            {
                throw new InvalidOperationException("El cuatrimestre debe estar entre 1 y 20.");
            }

            var degreeProgramExists = await _subjectManagementRepository.DegreeProgramExistsAsync(assignment.DegreeProgramId);
            if (!degreeProgramExists)
            {
                throw new InvalidOperationException("Una de las titulaciones seleccionadas no existe.");
            }
        }
    }

    private static SubjectType MapSubjectType(string type)
    {
        return type.Trim().ToLower() switch
        {
            "required" => SubjectType.Required,
            "optional" => SubjectType.Optional,
            "obligatoria" => SubjectType.Required,
            "optativa" => SubjectType.Optional,
            _ => throw new InvalidOperationException("El tipo de asignatura no es válido.")
        };
    }
}