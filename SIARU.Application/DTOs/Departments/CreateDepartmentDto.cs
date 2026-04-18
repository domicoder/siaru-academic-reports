using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.Departments;

public class CreateDepartmentDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(255, ErrorMessage = "El nombre no puede exceder los 255 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;
}