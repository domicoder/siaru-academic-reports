using SIARU.Application.DTOs.TeachingAssignments;
using SIARU.Application.Interfaces;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class TeachingAssignmentManagementService : ITeachingAssignmentManagementService
{
    private readonly ITeachingAssignmentManagementRepository _repository;
    private readonly IGenericRepository<KnowledgeArea> _knowledgeAreaRepository;

    public TeachingAssignmentManagementService(
        ITeachingAssignmentManagementRepository repository,
        IGenericRepository<KnowledgeArea> knowledgeAreaRepository)
    {
        _repository = repository;
        _knowledgeAreaRepository = knowledgeAreaRepository;
    }

    public async Task<List<TeachingAssignmentListDto>> GetAllAsync()
    {
        var items = await _repository.GetAllWithDetailsAsync();

        return items.Select(x => new TeachingAssignmentListDto
        {
            SubjectCode = x.SubjectCode,
            SubjectName = x.Subject.Name,
            ProfessorId = x.ProfessorId,
            ProfessorName = x.Professor.Name,
            KnowledgeAreaName = x.Subject.KnowledgeArea.Name,
            AcademicYear = x.AcademicYear
        }).ToList();
    }

    public async Task<TeachingAssignmentDetailDto?> GetByKeyAsync(string subjectCode, int professorId, string academicYear)
    {
        var item = await _repository.GetByKeyWithDetailsAsync(subjectCode, professorId, academicYear);

        if (item is null)
        {
            return null;
        }

        return new TeachingAssignmentDetailDto
        {
            SubjectCode = item.SubjectCode,
            SubjectName = item.Subject.Name,
            ProfessorId = item.ProfessorId,
            ProfessorName = item.Professor.Name,
            ProfessorOffice = item.Professor.Office,
            KnowledgeAreaName = item.Subject.KnowledgeArea.Name,
            AcademicYear = item.AcademicYear
        };
    }

    public async Task CreateAsync(CreateTeachingAssignmentDto dto)
    {
        var subjectCode = dto.SubjectCode.Trim();
        var academicYear = dto.AcademicYear.Trim();

        await ValidateAssignmentAsync(subjectCode, dto.ProfessorId, academicYear);

        var entity = new TeachingAssignment
        {
            SubjectCode = subjectCode,
            ProfessorId = dto.ProfessorId,
            AcademicYear = academicYear
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(UpdateTeachingAssignmentDto dto)
    {
        var original = await _repository.GetByKeyWithDetailsAsync(
            dto.OriginalSubjectCode,
            dto.OriginalProfessorId,
            dto.OriginalAcademicYear);

        if (original is null)
        {
            return false;
        }

        var newSubjectCode = dto.SubjectCode.Trim();
        var newAcademicYear = dto.AcademicYear.Trim();

        var sameKey =
            dto.OriginalSubjectCode == newSubjectCode &&
            dto.OriginalProfessorId == dto.ProfessorId &&
            dto.OriginalAcademicYear == newAcademicYear;

        if (!sameKey)
        {
            await ValidateAssignmentAsync(newSubjectCode, dto.ProfessorId, newAcademicYear);
        }
        else
        {
            await ValidateSubjectProfessorCompatibilityAsync(newSubjectCode, dto.ProfessorId);
        }

        var updatedEntity = new TeachingAssignment
        {
            SubjectCode = newSubjectCode,
            ProfessorId = dto.ProfessorId,
            AcademicYear = newAcademicYear
        };

        await _repository.UpdateAsync(original, updatedEntity);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteAsync(string subjectCode, int professorId, string academicYear)
    {
        var entity = await _repository.GetByKeyWithDetailsAsync(subjectCode, professorId, academicYear);

        if (entity is null)
        {
            return false;
        }

        await _repository.SoftDeleteAsync(entity);
        await _repository.SaveChangesAsync();

        return true;
    }

    private async Task ValidateAssignmentAsync(string subjectCode, int professorId, string academicYear)
    {
        if (string.IsNullOrWhiteSpace(subjectCode))
        {
            throw new InvalidOperationException("Debes seleccionar una asignatura válida.");
        }

        if (string.IsNullOrWhiteSpace(academicYear))
        {
            throw new InvalidOperationException("El año académico es obligatorio.");
        }

        var exists = await _repository.ExistsAsync(subjectCode, professorId, academicYear);
        if (exists)
        {
            throw new InvalidOperationException("Ya existe una impartición con la misma asignatura, profesor y año académico.");
        }

        await ValidateSubjectProfessorCompatibilityAsync(subjectCode, professorId);
    }

    private async Task ValidateSubjectProfessorCompatibilityAsync(string subjectCode, int professorId)
    {
        var subject = await _repository.GetSubjectAsync(subjectCode);
        if (subject is null || subject.IsDeleted)
        {
            throw new InvalidOperationException("La asignatura seleccionada no existe.");
        }

        var professor = await _repository.GetProfessorAsync(professorId);
        if (professor is null || professor.IsDeleted)
        {
            throw new InvalidOperationException("El profesor seleccionado no existe.");
        }

        if (subject.KnowledgeAreaId != professor.KnowledgeAreaId)
        {
            throw new InvalidOperationException("El profesor solo puede impartir asignaturas de su misma área de conocimiento.");
        }
    }
}