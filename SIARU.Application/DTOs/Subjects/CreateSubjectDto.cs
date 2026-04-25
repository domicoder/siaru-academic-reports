using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.Subjects;

public class CreateSubjectDto
{
    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(10, ErrorMessage = "El código no puede exceder los 10 caracteres.")]
    [Display(Name = "Código")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "El curso debe ser mayor que cero.")]
    [Display(Name = "Curso")]
    public int Course { get; set; }

    [Range(typeof(decimal), "0", "999", ErrorMessage = "Créditos teóricos inválidos.")]
    [Display(Name = "Créditos teóricos")]
    public decimal TheoreticalCredits { get; set; }

    [Range(typeof(decimal), "0", "999", ErrorMessage = "Créditos de laboratorio inválidos.")]
    [Display(Name = "Créditos de laboratorio")]
    public decimal LabCredits { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    [Display(Name = "Tipo")]
    public string Type { get; set; } = string.Empty;

    [Display(Name = "Límite de admisión")]
    [Range(0, int.MaxValue, ErrorMessage = "El límite de admisión no puede ser negativo.")]
    public int? AdmissionLimit { get; set; }

    [Required(ErrorMessage = "Debes seleccionar un área de conocimiento.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un área de conocimiento válida.")]
    [Display(Name = "Área de conocimiento")]
    public int KnowledgeAreaId { get; set; }

    public List<SubjectDegreeProgramAssignmentDto> DegreePrograms { get; set; } = new();
}