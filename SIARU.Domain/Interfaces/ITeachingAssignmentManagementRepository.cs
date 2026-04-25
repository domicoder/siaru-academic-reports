using SIARU.Domain.Entities;

namespace SIARU.Domain.Interfaces;

public interface ITeachingAssignmentManagementRepository
{
    Task<List<TeachingAssignment>> GetAllWithDetailsAsync();
    Task<TeachingAssignment?> GetByKeyWithDetailsAsync(string subjectCode, int professorId, string academicYear);
    Task<Subject?> GetSubjectAsync(string subjectCode);
    Task<Professor?> GetProfessorAsync(int professorId);
    Task<bool> ExistsAsync(string subjectCode, int professorId, string academicYear);
    Task AddAsync(TeachingAssignment entity);
    Task UpdateAsync(TeachingAssignment originalEntity, TeachingAssignment updatedEntity);
    Task SoftDeleteAsync(TeachingAssignment entity);
    Task<int> SaveChangesAsync();
}