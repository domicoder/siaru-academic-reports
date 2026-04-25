using SIARU.Application.DTOs.KnowledgeAreas;

namespace SIARU.Application.Interfaces;

public interface IKnowledgeAreaService
{
    Task<List<KnowledgeAreaListDto>> GetAllAsync();
    Task<KnowledgeAreaDetailDto?> GetByIdAsync(int id);
    Task<int> CreateAsync(CreateKnowledgeAreaDto dto);
    Task<bool> UpdateAsync(UpdateKnowledgeAreaDto dto);
    Task<bool> SoftDeleteAsync(int id);
}