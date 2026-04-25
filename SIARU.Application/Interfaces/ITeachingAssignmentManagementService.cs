using SIARU.Application.DTOs.TeachingAssignments;

namespace SIARU.Application.Interfaces;

public interface ITeachingAssignmentManagementService
{
    Task<List<TeachingAssignmentListDto>> GetAllAsync();
    Task<TeachingAssignmentDetailDto?> GetByKeyAsync(string subjectCode, int professorId, string academicYear);
    Task CreateAsync(CreateTeachingAssignmentDto dto);
    Task<bool> UpdateAsync(UpdateTeachingAssignmentDto dto);
    Task<bool> SoftDeleteAsync(string subjectCode, int professorId, string academicYear);
}