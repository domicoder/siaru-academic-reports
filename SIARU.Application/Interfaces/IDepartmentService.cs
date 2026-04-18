using SIARU.Application.DTOs.Departments;

namespace SIARU.Application.Interfaces;

public interface IDepartmentService
{
    Task<List<DepartmentListDto>> GetAllAsync();
    Task<DepartmentDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateDepartmentDto dto);
    Task<bool> UpdateAsync(UpdateDepartmentDto dto);
    Task<bool> SoftDeleteAsync(int id);
}