using SIARU.Domain.Common;
using SIARU.Domain.Enums;

namespace SIARU.Domain.Entities;

public class OfficeHour : AuditableSoftDeletableEntity
{
    public int Id { get; set; }

    public int ProfessorId { get; set; }
    public Professor Professor { get; set; } = null!;

    public WeekDay DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
}