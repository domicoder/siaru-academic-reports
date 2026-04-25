using Microsoft.EntityFrameworkCore;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;
using SIARU.Infrastructure.Persistence;

namespace SIARU.Infrastructure.Repositories;

public class TeachingAssignmentManagementRepository : ITeachingAssignmentManagementRepository
{
    private readonly SiaruDbContext _context;

    public TeachingAssignmentManagementRepository(SiaruDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeachingAssignment>> GetAllWithDetailsAsync()
    {
        return await _context.TeachingAssignments
            .Include(x => x.Subject)
                .ThenInclude(x => x.KnowledgeArea)
            .Include(x => x.Professor)
            .OrderBy(x => x.AcademicYear)
            .ThenBy(x => x.Subject.Name)
            .ThenBy(x => x.Professor.Name)
            .ToListAsync();
    }

    public async Task<TeachingAssignment?> GetByKeyWithDetailsAsync(string subjectCode, int professorId, string academicYear)
    {
        return await _context.TeachingAssignments
            .Include(x => x.Subject)
                .ThenInclude(x => x.KnowledgeArea)
            .Include(x => x.Professor)
            .FirstOrDefaultAsync(x =>
                x.SubjectCode == subjectCode &&
                x.ProfessorId == professorId &&
                x.AcademicYear == academicYear);
    }

    public async Task<Subject?> GetSubjectAsync(string subjectCode)
    {
        return await _context.Subjects.FirstOrDefaultAsync(x => x.Code == subjectCode);
    }

    public async Task<Professor?> GetProfessorAsync(int professorId)
    {
        return await _context.Professors.FirstOrDefaultAsync(x => x.Id == professorId);
    }

    public async Task<bool> ExistsAsync(string subjectCode, int professorId, string academicYear)
    {
        return await _context.TeachingAssignments.AnyAsync(x =>
            x.SubjectCode == subjectCode &&
            x.ProfessorId == professorId &&
            x.AcademicYear == academicYear);
    }

    public async Task AddAsync(TeachingAssignment entity)
    {
        await _context.TeachingAssignments.AddAsync(entity);
    }

    public async Task UpdateAsync(TeachingAssignment originalEntity, TeachingAssignment updatedEntity)
    {
        var keyChanged =
            originalEntity.SubjectCode != updatedEntity.SubjectCode ||
            originalEntity.ProfessorId != updatedEntity.ProfessorId ||
            originalEntity.AcademicYear != updatedEntity.AcademicYear;

        if (!keyChanged)
        {
            originalEntity.SubjectCode = updatedEntity.SubjectCode;
            originalEntity.ProfessorId = updatedEntity.ProfessorId;
            originalEntity.AcademicYear = updatedEntity.AcademicYear;

            _context.TeachingAssignments.Update(originalEntity);
            return;
        }

        _context.TeachingAssignments.Remove(originalEntity);
        await _context.SaveChangesAsync();

        await _context.TeachingAssignments.AddAsync(updatedEntity);
    }

    public async Task SoftDeleteAsync(TeachingAssignment entity)
    {
        _context.TeachingAssignments.Remove(entity);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}