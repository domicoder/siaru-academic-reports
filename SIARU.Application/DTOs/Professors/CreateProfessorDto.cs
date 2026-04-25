using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.Professors;

public class CreateProfessorDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El despacho es obligatorio.")]
    [StringLength(50, ErrorMessage = "El despacho no puede exceder los 50 caracteres.")]
    [Display(Name = "Despacho")]
    public string Office { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar un área de conocimiento.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un área de conocimiento válida.")]
    [Display(Name = "Área de conocimiento")]
    public int KnowledgeAreaId { get; set; }
}