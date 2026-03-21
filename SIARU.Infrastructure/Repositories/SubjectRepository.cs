using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;
using SIARU.Infrastructure.Persistence;

namespace SIARU.Infrastructure.Repositories;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(SiaruDbContext context) : base(context)
    {
    }

    public async Task<List<Subject>> SearchByCodeOrNameAsync(string term)
    {
        term = term.Trim().ToLower();

        return await DbSet
            .Where(x => x.Code.ToLower().Contains(term) || x.Name.ToLower().Contains(term))
            .OrderBy(x => x.Code)
            .ToListAsync();
    }

    public async Task<Subject?> GetReportByCodeAsync(string code)
    {
        return await DbSet
            .Include(x => x.KnowledgeArea)
                .ThenInclude(x => x.Department)
            .Include(x => x.SubjectDegreePrograms)
                .ThenInclude(x => x.DegreeProgram)
            .Include(x => x.TeachingAssignments.Where(t => !t.IsDeleted))
                .ThenInclude(x => x.Professor)
            .Include(x => x.Incompatibilities)
                .ThenInclude(x => x.IncompatibleWithSubject)
            .Include(x => x.Equivalencies)
                .ThenInclude(x => x.EquivalentToSubject)
            .FirstOrDefaultAsync(x => x.Code == code);
    }
}