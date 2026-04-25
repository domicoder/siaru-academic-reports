using System.ComponentModel.DataAnnotations;

namespace SIARU.Application.DTOs.Subjects;

public class SubjectDegreeProgramAssignmentDto
{
    [Required(ErrorMessage = "Debes seleccionar una titulación.")]
    [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar una titulación válida.")]
    public int DegreeProgramId { get; set; }

    [Range(1, 20, ErrorMessage = "El cuatrimestre debe estar entre 1 y 20.")]
    public int Quadrimester { get; set; }

    public bool IsFreeConfiguration { get; set; }
}