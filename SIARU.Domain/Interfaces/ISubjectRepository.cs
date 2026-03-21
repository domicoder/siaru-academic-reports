using SIARU.Domain.Entities;

namespace SIARU.Domain.Interfaces;

public interface ISubjectRepository : IGenericRepository<Subject>
{
    Task<List<Subject>> SearchByCodeOrNameAsync(string term);
    Task<Subject?> GetReportByCodeAsync(string code);
}