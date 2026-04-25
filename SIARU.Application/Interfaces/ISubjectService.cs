using SIARU.Application.DTOs.Subjects;

namespace SIARU.Application.Interfaces;

public interface ISubjectService
{
    Task<List<SubjectListDto>> GetAllAsync();
    Task<SubjectDetailDto?> GetByCodeAsync(string code);
    Task<string> CreateAsync(CreateSubjectDto dto);
    Task<bool> UpdateAsync(UpdateSubjectDto dto);
    Task<bool> SoftDeleteAsync(string code);
}