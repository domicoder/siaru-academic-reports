using SIARU.Application.DTOs.Professors;

namespace SIARU.Application.Interfaces;

public interface IProfessorManagementService
{
    Task<List<ProfessorListDto>> GetAllAsync();
    Task<ProfessorDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateProfessorDto dto);
    Task<bool> UpdateAsync(UpdateProfessorDto dto);
    Task<bool> SoftDeleteAsync(int id);
}