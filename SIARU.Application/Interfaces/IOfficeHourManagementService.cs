using SIARU.Application.DTOs.OfficeHours;

namespace SIARU.Application.Interfaces;

public interface IOfficeHourManagementService
{
    Task<List<OfficeHourListDto>> GetAllAsync();
    Task<OfficeHourDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateOfficeHourDto dto);
    Task<bool> UpdateAsync(UpdateOfficeHourDto dto);
    Task<bool> SoftDeleteAsync(int id);
}