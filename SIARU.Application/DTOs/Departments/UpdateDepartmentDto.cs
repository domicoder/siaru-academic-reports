using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.Departments;

public class UpdateDepartmentDto
{
    [Required]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;
}