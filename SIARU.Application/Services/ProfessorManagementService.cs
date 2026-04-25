using SIARU.Application.DTOs.Professors;
using SIARU.Application.Interfaces;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class ProfessorManagementService : IProfessorManagementService
{
    private readonly IGenericRepository<Professor> _professorRepository;
    private readonly IGenericRepository<KnowledgeArea> _knowledgeAreaRepository;
    private readonly IGenericRepository<Department> _departmentRepository;

    public ProfessorManagementService(
        IGenericRepository<Professor> professorRepository,
        IGenericRepository<KnowledgeArea> knowledgeAreaRepository,
        IGenericRepository<Department> departmentRepository)
    {
        _professorRepository = professorRepository;
        _knowledgeAreaRepository = knowledgeAreaRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<List<ProfessorListDto>> GetAllAsync()
    {
        var professors = await _professorRepository.GetAllAsync();
        var knowledgeAreas = await _knowledgeAreaRepository.GetAllAsync();
        var departments = await _departmentRepository.GetAllAsync();

        var knowledgeAreaMap = knowledgeAreas.ToDictionary(x => x.Id);
        var departmentMap = departments.ToDictionary(x => x.Id, x => x.Name);

        return professors
            .OrderBy(x => x.Name)
            .Select(x =>
            {
                knowledgeAreaMap.TryGetValue(x.KnowledgeAreaId, out var knowledgeArea);
                var departmentName = knowledgeArea is not null && departmentMap.TryGetValue(knowledgeArea.DepartmentId, out var foundDepartmentName)
                    ? foundDepartmentName
                    : "Departamento no disponible";

                return new ProfessorListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Office = x.Office,
                    KnowledgeAreaId = x.KnowledgeAreaId,
                    KnowledgeAreaName = knowledgeArea?.Name ?? "Área no disponible",
                    DepartmentName = departmentName
                };
            })
            .ToList();
    }

    public async Task<ProfessorDetailDto?> GetByIdAsync(int id)
    {
        var professor = await _professorRepository.GetByIdAsync(id);

        if (professor is null || professor.IsDeleted)
        {
            return null;
        }

        var knowledgeArea = await _knowledgeAreaRepository.GetByIdAsync(professor.KnowledgeAreaId);
        var department = knowledgeArea is not null
            ? await _departmentRepository.GetByIdAsync(knowledgeArea.DepartmentId)
            : null;

        return new ProfessorDetailDto
        {
            Id = professor.Id,
            Name = professor.Name,
            Office = professor.Office,
            KnowledgeAreaId = professor.KnowledgeAreaId,
            KnowledgeAreaName = knowledgeArea?.Name ?? "Área no disponible",
            DepartmentName = department?.Name ?? "Departamento no disponible"
        };
    }

    public async Task<int> CreateAsync(CreateProfessorDto dto)
    {
        var normalizedName = dto.Name.Trim();
        var normalizedOffice = dto.Office.Trim().ToUpperInvariant();

        var knowledgeArea = await _knowledgeAreaRepository.GetByIdAsync(dto.KnowledgeAreaId);
        if (knowledgeArea is null || knowledgeArea.IsDeleted)
        {
            throw new InvalidOperationException("El área de conocimiento seleccionada no existe.");
        }

        var duplicateExists = await _professorRepository.ExistsAsync(x =>
            x.Name.ToLower() == normalizedName.ToLower() &&
            x.KnowledgeAreaId == dto.KnowledgeAreaId);

        if (duplicateExists)
        {
            throw new InvalidOperationException("Ya existe un profesor con ese nombre en el área de conocimiento seleccionada.");
        }

        var entity = new Professor
        {
            Name = normalizedName,
            Office = normalizedOffice,
            KnowledgeAreaId = dto.KnowledgeAreaId
        };

        await _professorRepository.AddAsync(entity);
        await _professorRepository.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(UpdateProfessorDto dto)
    {
        var professor = await _professorRepository.GetByIdAsync(dto.Id);

        if (professor is null || professor.IsDeleted)
        {
            return false;
        }

        var normalizedName = dto.Name.Trim();
        var normalizedOffice = dto.Office.Trim().ToUpperInvariant();

        var knowledgeArea = await _knowledgeAreaRepository.GetByIdAsync(dto.KnowledgeAreaId);
        if (knowledgeArea is null || knowledgeArea.IsDeleted)
        {
            throw new InvalidOperationException("El área de conocimiento seleccionada no existe.");
        }

        var duplicateExists = await _professorRepository.ExistsAsync(x =>
            x.Id != dto.Id &&
            x.Name.ToLower() == normalizedName.ToLower() &&
            x.KnowledgeAreaId == dto.KnowledgeAreaId);

        if (duplicateExists)
        {
            throw new InvalidOperationException("Ya existe un profesor con ese nombre en el área de conocimiento seleccionada.");
        }

        professor.Name = normalizedName;
        professor.Office = normalizedOffice;
        professor.KnowledgeAreaId = dto.KnowledgeAreaId;

        _professorRepository.Update(professor);
        await _professorRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var professor = await _professorRepository.GetByIdAsync(id);

        if (professor is null || professor.IsDeleted)
        {
            return false;
        }

        _professorRepository.Remove(professor);
        await _professorRepository.SaveChangesAsync();

        return true;
    }
}