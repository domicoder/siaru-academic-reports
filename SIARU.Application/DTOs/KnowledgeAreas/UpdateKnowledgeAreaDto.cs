using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.KnowledgeAreas;

public class UpdateKnowledgeAreaDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes seleccionar un departamento.")]
    [Display(Name = "Departamento")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un departamento válido.")]
    public int DepartmentId { get; set; }
}