using SIARU.Application.DTOs.OfficeHours;
using SIARU.Application.Interfaces;
using SIARU.Domain.Entities;
using SIARU.Domain.Enums;
using SIARU.Domain.Interfaces;

namespace SIARU.Application.Services;

public class OfficeHourManagementService : IOfficeHourManagementService
{
    private readonly IGenericRepository<OfficeHour> _officeHourRepository;
    private readonly IGenericRepository<Professor> _professorRepository;
    private readonly IGenericRepository<KnowledgeArea> _knowledgeAreaRepository;

    public OfficeHourManagementService(
        IGenericRepository<OfficeHour> officeHourRepository,
        IGenericRepository<Professor> professorRepository,
        IGenericRepository<KnowledgeArea> knowledgeAreaRepository)
    {
        _officeHourRepository = officeHourRepository;
        _professorRepository = professorRepository;
        _knowledgeAreaRepository = knowledgeAreaRepository;
    }

    public async Task<List<OfficeHourListDto>> GetAllAsync()
    {
        var officeHours = await _officeHourRepository.GetAllAsync();
        var professors = await _professorRepository.GetAllAsync();
        var knowledgeAreas = await _knowledgeAreaRepository.GetAllAsync();

        var professorMap = professors.ToDictionary(x => x.Id);
        var knowledgeAreaMap = knowledgeAreas.ToDictionary(x => x.Id, x => x.Name);

        return officeHours
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .Select(x =>
            {
                professorMap.TryGetValue(x.ProfessorId, out var professor);

                var knowledgeAreaName = professor is not null && knowledgeAreaMap.TryGetValue(professor.KnowledgeAreaId, out var areaName)
                    ? areaName
                    : "Área no disponible";

                return new OfficeHourListDto
                {
                    Id = x.Id,
                    ProfessorId = x.ProfessorId,
                    ProfessorName = professor?.Name ?? "Profesor no disponible",
                    ProfessorOffice = professor?.Office ?? "N/D",
                    KnowledgeAreaName = knowledgeAreaName,
                    DayOfWeek = x.DayOfWeek.ToString(),
                    StartTime = x.StartTime.ToString("HH\\:mm"),
                    EndTime = x.EndTime.ToString("HH\\:mm")
                };
            })
            .ToList();
    }

    public async Task<OfficeHourDetailDto?> GetByIdAsync(int id)
    {
        var officeHour = await _officeHourRepository.GetByIdAsync(id);

        if (officeHour is null || officeHour.IsDeleted)
        {
            return null;
        }

        var professor = await _professorRepository.GetByIdAsync(officeHour.ProfessorId);
        var knowledgeArea = professor is not null
            ? await _knowledgeAreaRepository.GetByIdAsync(professor.KnowledgeAreaId)
            : null;

        return new OfficeHourDetailDto
        {
            Id = officeHour.Id,
            ProfessorId = officeHour.ProfessorId,
            ProfessorName = professor?.Name ?? "Profesor no disponible",
            ProfessorOffice = professor?.Office ?? "N/D",
            KnowledgeAreaName = knowledgeArea?.Name ?? "Área no disponible",
            DayOfWeek = officeHour.DayOfWeek.ToString(),
            StartTime = officeHour.StartTime.ToString("HH\\:mm"),
            EndTime = officeHour.EndTime.ToString("HH\\:mm")
        };
    }

    public async Task<int> CreateAsync(CreateOfficeHourDto dto)
    {
        var professor = await _professorRepository.GetByIdAsync(dto.ProfessorId);
        if (professor is null || professor.IsDeleted)
        {
            throw new InvalidOperationException("El profesor seleccionado no existe.");
        }

        var dayOfWeek = MapWeekDay(dto.DayOfWeek);
        var startTime = ParseTime(dto.StartTime);
        var endTime = ParseTime(dto.EndTime);

        ValidateTimeRange(startTime, endTime);

        var entity = new OfficeHour
        {
            ProfessorId = dto.ProfessorId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime
        };

        await _officeHourRepository.AddAsync(entity);
        await _officeHourRepository.SaveChangesAsync();

        return entity.Id;
    }

    public async Task<bool> UpdateAsync(UpdateOfficeHourDto dto)
    {
        var officeHour = await _officeHourRepository.GetByIdAsync(dto.Id);

        if (officeHour is null || officeHour.IsDeleted)
        {
            return false;
        }

        var professor = await _professorRepository.GetByIdAsync(dto.ProfessorId);
        if (professor is null || professor.IsDeleted)
        {
            throw new InvalidOperationException("El profesor seleccionado no existe.");
        }

        var dayOfWeek = MapWeekDay(dto.DayOfWeek);
        var startTime = ParseTime(dto.StartTime);
        var endTime = ParseTime(dto.EndTime);

        ValidateTimeRange(startTime, endTime);

        officeHour.ProfessorId = dto.ProfessorId;
        officeHour.DayOfWeek = dayOfWeek;
        officeHour.StartTime = startTime;
        officeHour.EndTime = endTime;

        _officeHourRepository.Update(officeHour);
        await _officeHourRepository.SaveChangesAsync();

        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var officeHour = await _officeHourRepository.GetByIdAsync(id);

        if (officeHour is null || officeHour.IsDeleted)
        {
            return false;
        }

        _officeHourRepository.Remove(officeHour);
        await _officeHourRepository.SaveChangesAsync();

        return true;
    }

    private static void ValidateTimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
        {
            throw new InvalidOperationException("La hora de fin debe ser mayor que la hora de inicio.");
        }
    }

    private static TimeOnly ParseTime(string value)
    {
        if (!TimeOnly.TryParse(value, out var time))
        {
            throw new InvalidOperationException("El formato de hora no es válido.");
        }

        return time;
    }

    private static WeekDay MapWeekDay(string value)
    {
        return value.Trim().ToLower() switch
        {
            "monday" or "lunes" => WeekDay.Monday,
            "tuesday" or "martes" => WeekDay.Tuesday,
            "wednesday" or "miércoles" or "miercoles" => WeekDay.Wednesday,
            "thursday" or "jueves" => WeekDay.Thursday,
            "friday" or "viernes" => WeekDay.Friday,
            "saturday" or "sábado" or "sabado" => WeekDay.Saturday,
            "sunday" or "domingo" => WeekDay.Sunday,
            _ => throw new InvalidOperationException("El día de la semana no es válido.")
        };
    }
}