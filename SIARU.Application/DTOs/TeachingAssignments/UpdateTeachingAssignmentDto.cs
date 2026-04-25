using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.TeachingAssignments;

public class UpdateTeachingAssignmentDto
{
    [Required]
    public string OriginalSubjectCode { get; set; } = string.Empty;

    [Required]
    public int OriginalProfessorId { get; set; }

    [Required]
    public string OriginalAcademicYear { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar una asignatura.")]
    [Display(Name = "Asignatura")]
    public string SubjectCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar un profesor.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un profesor válido.")]
    [Display(Name = "Profesor")]
    public int ProfessorId { get; set; }

    [Required(ErrorMessage = "El año académico es obligatorio.")]
    [RegularExpression(@"^\d{4}/\d{2}$", ErrorMessage = "El año académico debe tener el formato YYYY/YY. Ejemplo: 2025/26")]
    [Display(Name = "Año académico")]
    public string AcademicYear { get; set; } = string.Empty;
}