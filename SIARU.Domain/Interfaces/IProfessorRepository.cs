using SIARU.Domain.Entities;

namespace SIARU.Domain.Interfaces;

public interface IProfessorRepository : IGenericRepository<Professor>
{
    Task<List<Professor>> SearchByNameAsync(string term);
    Task<Professor?> GetReportByIdAsync(int id);
}