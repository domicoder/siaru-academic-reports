using SIARU.Application.DTOs.DegreePrograms;
using SIARU.Application.Interfaces;
using SIARU.Domain.Entities;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class DegreeProgramService : IDegreeProgramService
{
    private readonly IGenericRepository<DegreeProgram> _degreeProgramRepository;

    public DegreeProgramService(IGenericRepository<DegreeProgram> degreeProgramRepository)
    {
        _degreeProgramRepository = degreeProgramRepository;
    }

    public async Task<List<DegreeProgramListDto>> GetAllAsync()
    {
        var items = await _degreeProgramRepository.GetAllAsync();

        return items
            .OrderBy(x => x.Name)
            .Select(x => new DegreeProgramListDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToList();
    }

    public async Task<DegreeProgramDetailDto?> GetByIdAsync(int id)
    {
        var item = await _degreeProgramRepository.GetByIdAsync(id);

        if (item is null || item.IsDeleted)
        {
            return null;
        }

        return new DegreeProgramDetailDto
        {
            Id = item.Id,
            Name = item.Name
        };
    }

    public async Task<int> CreateAsync(CreateDegreeProgramDto dto)
    {
        var normalizedName = dto.Name.Trim();

        var exists = await _degreeProgramRepository.ExistsAsync(x =>
            x.Name.ToLower() == normalizedName.ToLower());

        if (exists)
        {
            throw new InvalidOperationException("Ya existe una titulación con ese nombre.");
        }

        var entity = new DegreeProgram
        {
            Name = normalizedName
        };

        await _degreeProgramRepository.AddAsync(entity);
        await _degreeProgramRepository.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(UpdateDegreeProgramDto dto)
    {
        var item = await _degreeProgramRepository.GetByIdAsync(dto.Id);

        if (item is null || item.IsDeleted)
        {
            return false;
        }

        var normalizedName = dto.Name.Trim();

        var duplicateExists = await _degreeProgramRepository.ExistsAsync(x =>
            x.Id != dto.Id &&
            x.Name.ToLower() == normalizedName.ToLower());

        if (duplicateExists)
        {
            throw new InvalidOperationException("Ya existe una titulación con ese nombre.");
        }

        item.Name = normalizedName;

        _degreeProgramRepository.Update(item);
        await _degreeProgramRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var item = await _degreeProgramRepository.GetByIdAsync(id);

        if (item is null || item.IsDeleted)
        {
            return false;
        }

        _degreeProgramRepository.Remove(item);
        await _degreeProgramRepository.SaveChangesAsync();

        return true;
    }
}