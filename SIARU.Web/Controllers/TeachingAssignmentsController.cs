using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIARU.Application.Common;
using SIARU.Application.DTOs.TeachingAssignments;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff}")]
public class TeachingAssignmentsController : Controller
{
    private readonly ITeachingAssignmentManagementService _service;
    private readonly ISubjectService _subjectService;
    private readonly IProfessorManagementService _professorService;

    public TeachingAssignmentsController(
        ITeachingAssignmentManagementService service,
        ISubjectService subjectService,
        IProfessorManagementService professorService)
    {
        _service = service;
        _subjectService = subjectService;
        _professorService = professorService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _service.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(string subjectCode, int professorId, string academicYear)
    {
        var item = await _service.GetByKeyAsync(subjectCode, professorId, academicYear);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadSelectorsAsync();
        return View(new CreateTeachingAssignmentDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateTeachingAssignmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectorsAsync(dto.SubjectCode, dto.ProfessorId);
            return View(dto);
        }

        try
        {
            await _service.CreateAsync(dto);
            TempData["SuccessMessage"] = "Impartición creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectorsAsync(dto.SubjectCode, dto.ProfessorId);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string subjectCode, int professorId, string academicYear)
    {
        var item = await _service.GetByKeyAsync(subjectCode, professorId, academicYear);

        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateTeachingAssignmentDto
        {
            OriginalSubjectCode = item.SubjectCode,
            OriginalProfessorId = item.ProfessorId,
            OriginalAcademicYear = item.AcademicYear,
            SubjectCode = item.SubjectCode,
            ProfessorId = item.ProfessorId,
            AcademicYear = item.AcademicYear
        };

        await LoadSelectorsAsync(dto.SubjectCode, dto.ProfessorId);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateTeachingAssignmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectorsAsync(dto.SubjectCode, dto.ProfessorId);
            return View(dto);
        }

        try
        {
            var updated = await _service.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Impartición actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectorsAsync(dto.SubjectCode, dto.ProfessorId);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string subjectCode, int professorId, string academicYear)
    {
        var item = await _service.GetByKeyAsync(subjectCode, professorId, academicYear);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string subjectCode, int professorId, string academicYear)
    {
        var deleted = await _service.SoftDeleteAsync(subjectCode, professorId, academicYear);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Impartición eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSelectorsAsync(string? selectedSubjectCode = null, int? selectedProfessorId = null)
    {
        var subjects = await _subjectService.GetAllAsync();
        var professors = await _professorService.GetAllAsync();

        ViewBag.Subjects = subjects
            .Select(x => new SelectListItem
            {
                Value = x.Code,
                Text = $"{x.Code} - {x.Name} ({x.KnowledgeAreaName})",
                Selected = selectedSubjectCode == x.Code
            })
            .ToList();

        ViewBag.Professors = professors
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} ({x.KnowledgeAreaName})",
                Selected = selectedProfessorId.HasValue && x.Id == selectedProfessorId.Value
            })
            .ToList();
    }
}