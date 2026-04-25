using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIARU.Application.Common;
using SIARU.Application.DTOs.Professors;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff}")]
public class ProfessorsController : Controller
{
    private readonly IProfessorManagementService _professorManagementService;
    private readonly IKnowledgeAreaService _knowledgeAreaService;

    public ProfessorsController(
        IProfessorManagementService professorManagementService,
        IKnowledgeAreaService knowledgeAreaService)
    {
        _professorManagementService = professorManagementService;
        _knowledgeAreaService = knowledgeAreaService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _professorManagementService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _professorManagementService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadKnowledgeAreasAsync();
        return View(new CreateProfessorDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProfessorDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadKnowledgeAreasAsync(dto.KnowledgeAreaId);
            return View(dto);
        }

        try
        {
            await _professorManagementService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Profesor creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadKnowledgeAreasAsync(dto.KnowledgeAreaId);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _professorManagementService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateProfessorDto
        {
            Id = item.Id,
            Name = item.Name,
            Office = item.Office,
            KnowledgeAreaId = item.KnowledgeAreaId
        };

        await LoadKnowledgeAreasAsync(dto.KnowledgeAreaId);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateProfessorDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadKnowledgeAreasAsync(dto.KnowledgeAreaId);
            return View(dto);
        }

        try
        {
            var updated = await _professorManagementService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Profesor actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadKnowledgeAreasAsync(dto.KnowledgeAreaId);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _professorManagementService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var deleted = await _professorManagementService.SoftDeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Profesor eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadKnowledgeAreasAsync(int? selectedKnowledgeAreaId = null)
    {
        var knowledgeAreas = await _knowledgeAreaService.GetAllAsync();

        ViewBag.KnowledgeAreas = knowledgeAreas
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} ({x.DepartmentName})",
                Selected = selectedKnowledgeAreaId.HasValue && x.Id == selectedKnowledgeAreaId.Value
            })
            .ToList();
    }
}