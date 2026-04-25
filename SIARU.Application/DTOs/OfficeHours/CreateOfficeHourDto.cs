using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.OfficeHours;

public class CreateOfficeHourDto
{
    [Required(ErrorMessage = "Debes seleccionar un profesor.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un profesor válido.")]
    [Display(Name = "Profesor")]
    public int ProfessorId { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un día.")]
    [Display(Name = "Día de la semana")]
    public string DayOfWeek { get; set; } = string.Empty;

    [Required(ErrorMessage = "La hora de inicio es obligatoria.")]
    [Display(Name = "Hora de inicio")]
    public string StartTime { get; set; } = string.Empty;

    [Required(ErrorMessage = "La hora de fin es obligatoria.")]
    [Display(Name = "Hora de fin")]
    public string EndTime { get; set; } = string.Empty;
}