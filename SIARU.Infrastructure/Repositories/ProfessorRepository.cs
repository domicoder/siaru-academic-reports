using Microsoft.EntityFrameworkCore;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;
using SIARU.Infrastructure.Persistence;

namespace SIARU.Infrastructure.Repositories;

public class ProfessorRepository : GenericRepository<Professor>, IProfessorRepository
{
    public ProfessorRepository(SiaruDbContext context) : base(context)
    {
    }

    public async Task<List<Professor>> SearchByNameAsync(string term)
    {
        term = term.Trim().ToLower();

        return await DbSet
            .Where(x => x.Name.ToLower().Contains(term))
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Professor?> GetReportByIdAsync(int id)
    {
        return await DbSet
            .Include(x => x.KnowledgeArea)
                .ThenInclude(x => x.Department)
            .Include(x => x.OfficeHours.Where(h => !h.IsDeleted))
            .Include(x => x.TeachingAssignments.Where(t => !t.IsDeleted))
                .ThenInclude(x => x.Subject)
            .FirstOrDefaultAsync(x => x.Id == id);
    }
}