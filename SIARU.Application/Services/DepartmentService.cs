using SIARU.Application.DTOs.Departments;
using SIARU.Application.Interfaces;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IGenericRepository<Department> _departmentRepository;

    public DepartmentService(IGenericRepository<Department> departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<List<DepartmentListDto>> GetAllAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();

        return departments
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentListDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();
    }

    public async Task<DepartmentDetailDto?> GetByIdAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null || department.IsDeleted)
        {
            return null;
        }

        return new DepartmentDetailDto
        {
            Id = department.Id,
            Name = department.Name
        };
    }

    public async Task<int> CreateAsync(CreateDepartmentDto dto)
    {
        var normalizedName = dto.Name.Trim();

        var exists = await _departmentRepository.ExistsAsync(x =>
            x.Name.ToLower() == normalizedName.ToLower());

        if (exists)
        {
            throw new InvalidOperationException("Ya existe un departamento con ese nombre.");
        }

        var entity = new Department
        {
            Name = normalizedName
        };

        await _departmentRepository.AddAsync(entity);
        await _departmentRepository.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(UpdateDepartmentDto dto)
    {
        var department = await _departmentRepository.GetByIdAsync(dto.Id);

        if (department is null || department.IsDeleted)
        {
            return false;
        }

        var normalizedName = dto.Name.Trim();

        var duplicateExists = await _departmentRepository.ExistsAsync(x =>
            x.Id != dto.Id &&
            x.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
        {
            throw new InvalidOperationException("Ya existe un departamento con ese nombre.");
        }

        department.Name = normalizedName;
        _departmentRepository.Update(department);
        await _departmentRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);

        if (department is null || department.IsDeleted)
        {
            return false;
        }

        _departmentRepository.Remove(department);
        await _departmentRepository.SaveChangesAsync();

        return true;
    }
}