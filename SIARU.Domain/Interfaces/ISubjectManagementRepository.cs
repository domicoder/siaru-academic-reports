using SIARU.Domain.Entities;

namespace SIARU.Domain.Interfaces;

public interface ISubjectManagementRepository
{
    Task<List<Subject>> GetAllWithKnowledgeAreaAsync();
    Task<Subject?> GetByCodeWithDetailsAsync(string code);
    Task<bool> CodeExistsAsync(string code);
    Task<bool> KnowledgeAreaExistsAsync(int knowledgeAreaId);
    Task<bool> DegreeProgramExistsAsync(int degreeProgramId);
    Task AddAsync(Subject subject, List<SubjectDegreeProgram> degreeProgramAssignments);
    Task UpdateAsync(Subject subject, List<SubjectDegreeProgram> degreeProgramAssignments);
    Task SoftDeleteAsync(Subject subject);
    Task<int> SaveChangesAsync();
}