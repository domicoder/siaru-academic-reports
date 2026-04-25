using Microsoft.EntityFrameworkCore;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;
using SIARU.Infrastructure.Persistence;

namespace SIARU.Infrastructure.Repositories;

public class SubjectManagementRepository : ISubjectManagementRepository
{
    private readonly SiaruDbContext _context;

    public SubjectManagementRepository(SiaruDbContext context)
    {
        _context = context;
    }

    public async Task<List<Subject>> GetAllWithKnowledgeAreaAsync()
    {
        return await _context.Subjects
            .Include(x => x.KnowledgeArea)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Subject?> GetByCodeWithDetailsAsync(string code)
    {
        return await _context.Subjects
            .Include(x => x.KnowledgeArea)
            .Include(x => x.SubjectDegreePrograms)
                .ThenInclude(x => x.DegreeProgram)
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<bool> CodeExistsAsync(string code)
    {
        return await _context.Subjects.AnyAsync(x => x.Code.ToLower() == code.ToLower());
    }

    public async Task<bool> KnowledgeAreaExistsAsync(int knowledgeAreaId)
    {
        return await _context.KnowledgeAreas.AnyAsync(x => x.Id == knowledgeAreaId && !x.IsDeleted);
    }

    public async Task<bool> DegreeProgramExistsAsync(int degreeProgramId)
    {
        return await _context.DegreePrograms.AnyAsync(x => x.Id == degreeProgramId && !x.IsDeleted);
    }

    public async Task AddAsync(Subject subject, List<SubjectDegreeProgram> degreeProgramAssignments)
    {
        await _context.Subjects.AddAsync(subject);
        await _context.SubjectDegreePrograms.AddRangeAsync(degreeProgramAssignments);
    }

    public async Task UpdateAsync(Subject subject, List<SubjectDegreeProgram> degreeProgramAssignments)
    {
        var existingAssignments = await _context.SubjectDegreePrograms
            .Where(x => x.SubjectCode == subject.Code)
            .ToListAsync();

        _context.SubjectDegreePrograms.RemoveRange(existingAssignments);

        _context.Subjects.Update(subject);
        await _context.SubjectDegreePrograms.AddRangeAsync(degreeProgramAssignments);
    }

    public async Task SoftDeleteAsync(Subject subject)
    {
        _context.Subjects.Update(subject);
        await Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}