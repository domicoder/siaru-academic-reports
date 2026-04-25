using SIARU.Application.DTOs.DegreePrograms;

namespace SIARU.Application.Interfaces;

public interface IDegreeProgramService
{
    Task<List<DegreeProgramListDto>> GetAllAsync();
    Task<DegreeProgramDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateDegreeProgramDto dto);
    Task<bool> UpdateAsync(UpdateDegreeProgramDto dto);
    Task<bool> SoftDeleteAsync(int id);
}