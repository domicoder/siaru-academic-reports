using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIARU.Application.Common;
using SIARU.Application.DTOs.KnowledgeAreas;
using SIARU.Application.DTOs.DegreePrograms;
using SIARU.Application.DTOs.Subjects;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff}")]
public class SubjectsController : Controller
{
    private readonly ISubjectService _subjectService;
    private readonly IKnowledgeAreaService _knowledgeAreaService;
    private readonly IDegreeProgramService _degreeProgramService;

    public SubjectsController(
        ISubjectService subjectService,
        IKnowledgeAreaService knowledgeAreaService,
        IDegreeProgramService degreeProgramService)
    {
        _subjectService = subjectService;
        _knowledgeAreaService = knowledgeAreaService;
        _degreeProgramService = degreeProgramService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _subjectService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(string id)
    {
        var item = await _subjectService.GetByCodeAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var dto = new CreateSubjectDto
        {
            DegreePrograms =
            [
                new SubjectDegreeProgramAssignmentDto()
            ]
        };

        await LoadSelectorsAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSubjectDto dto)
    {
        NormalizeDegreeProgramRows(dto.DegreePrograms);

        if (!ModelState.IsValid)
        {
            await LoadSelectorsAsync();
            return View(dto);
        }

        try
        {
            await _subjectService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Asignatura creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectorsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var item = await _subjectService.GetByCodeAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateSubjectDto
        {
            Code = item.Code,
            Name = item.Name,
            Course = item.Course,
            TheoreticalCredits = item.TheoreticalCredits,
            LabCredits = item.LabCredits,
            Type = item.Type,
            AdmissionLimit = item.AdmissionLimit,
            KnowledgeAreaId = item.KnowledgeAreaId,
            DegreePrograms = item.DegreePrograms
        };

        if (dto.DegreePrograms.Count == 0)
        {
            dto.DegreePrograms.Add(new SubjectDegreeProgramAssignmentDto());
        }

        await LoadSelectorsAsync();
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateSubjectDto dto)
    {
        NormalizeDegreeProgramRows(dto.DegreePrograms);

        if (!ModelState.IsValid)
        {
            await LoadSelectorsAsync();
            return View(dto);
        }

        try
        {
            var updated = await _subjectService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Asignatura actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectorsAsync();
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var item = await _subjectService.GetByCodeAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string code)
    {
        var deleted = await _subjectService.SoftDeleteAsync(code);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Asignatura eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSelectorsAsync()
    {
        var knowledgeAreas = await _knowledgeAreaService.GetAllAsync();
        var degreePrograms = await _degreeProgramService.GetAllAsync();

        ViewBag.KnowledgeAreas = knowledgeAreas
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} ({x.DepartmentName})"
            })
            .ToList();

        ViewBag.DegreePrograms = degreePrograms
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            })
            .ToList();

        ViewBag.SubjectTypes = new List<SelectListItem>
        {
            new() { Value = "Required", Text = "Obligatoria" },
            new() { Value = "Optional", Text = "Optativa" }
        };
    }

    private static void NormalizeDegreeProgramRows(List<SubjectDegreeProgramAssignmentDto>? rows)
    {
        if (rows is null)
        {
            return;
        }

        rows.RemoveAll(x => x.DegreeProgramId <= 0 && x.Quadrimester <= 0 && !x.IsFreeConfiguration);
    }
}