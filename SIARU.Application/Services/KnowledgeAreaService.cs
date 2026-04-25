using SIARU.Application.DTOs.KnowledgeAreas;
using SIARU.Application.Interfaces;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class KnowledgeAreaService : IKnowledgeAreaService
{
    private readonly IGenericRepository<KnowledgeArea> _knowledgeAreaRepository;
    private readonly IGenericRepository<Department> _departmentRepository;

    public KnowledgeAreaService(
        IGenericRepository<KnowledgeArea> knowledgeAreaRepository,
        IGenericRepository<Department> departmentRepository)
    {
        _knowledgeAreaRepository = knowledgeAreaRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<List<KnowledgeAreaListDto>> GetAllAsync()
    {
        var knowledgeAreas = await _knowledgeAreaRepository.GetAllAsync();
        var departments = await _departmentRepository.GetAllAsync();

        var departmentMap = departments.ToDictionary(x => x.Id, x => x.Name);

        return knowledgeAreas
            .OrderBy(x => x.Name)
            .Select(x => new KnowledgeAreaListDto
            {
                Id = x.Id,
                Name = x.Name,
                DepartmentId = x.DepartmentId,
                DepartmentName = departmentMap.TryGetValue(x.DepartmentId, out var departmentName)
                    ? departmentName
                    : "Departamento no disponible"
            })
            .ToList();
    }

    public async Task<KnowledgeAreaDetailDto?> GetByIdAsync(int id)
    {
        var knowledgeArea = await _knowledgeAreaRepository.GetByIdAsync(id);

        if (knowledgeArea is null || knowledgeArea.IsDeleted)
        {
            return null;
        }

        var department = await _departmentRepository.GetByIdAsync(knowledgeArea.DepartmentId);

        return new KnowledgeAreaDetailDto
        {
            Id = knowledgeArea.Id,
            Name = knowledgeArea.Name,
            DepartmentId = knowledgeArea.DepartmentId,
            DepartmentName = department?.Name ?? "Departamento no disponible"
        };
    }

    public async Task<int> CreateAsync(CreateKnowledgeAreaDto dto)
    {
        var normalizedName = dto.Name.Trim();

        var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
        if (department is null || department.IsDeleted)
        {
            throw new InvalidOperationException("El departamento seleccionado no existe.");
        }

        var duplicateExists = await _knowledgeAreaRepository.ExistsAsync(x =>
            x.DepartmentId == dto.DepartmentId &&
            x.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
        {
            throw new InvalidOperationException("Ya existe un área de conocimiento con ese nombre en el departamento seleccionado.");
        }

        var entity = new KnowledgeArea
        {
            Name = normalizedName,
            DepartmentId = dto.DepartmentId
        };

        await _knowledgeAreaRepository.AddAsync(entity);
        await _knowledgeAreaRepository.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(UpdateKnowledgeAreaDto dto)
    {
        var knowledgeArea = await _knowledgeAreaRepository.GetByIdAsync(dto.Id);

        if (knowledgeArea is null || knowledgeArea.IsDeleted)
        {
            return false;
        }

        var normalizedName = dto.Name.Trim();

        var department = await _departmentRepository.GetByIdAsync(dto.DepartmentId);
        if (department is null || department.IsDeleted)
        {
            throw new InvalidOperationException("El departamento seleccionado no existe.");
        }

        var duplicateExists = await _knowledgeAreaRepository.ExistsAsync(x =>
            x.Id != dto.Id &&
            x.DepartmentId == dto.DepartmentId &&
            x.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
        {
            throw new InvalidOperationException("Ya existe un área de conocimiento con ese nombre en el departamento seleccionado.");
        }

        knowledgeArea.Name = normalizedName;
        knowledgeArea.DepartmentId = dto.DepartmentId;

        _knowledgeAreaRepository.Update(knowledgeArea);
        await _knowledgeAreaRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var knowledgeArea = await _knowledgeAreaRepository.GetByIdAsync(id);

        if (knowledgeArea is null || knowledgeArea.IsDeleted)
        {
            return false;
        }

        _knowledgeAreaRepository.Remove(knowledgeArea);
        await _knowledgeAreaRepository.SaveChangesAsync();

        return true;
    }
}