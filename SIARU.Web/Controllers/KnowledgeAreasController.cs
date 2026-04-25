using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIARU.Application.Common;
using SIARU.Application.DTOs.Departments;
using SIARU.Application.DTOs.KnowledgeAreas;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff}")]
public class KnowledgeAreasController : Controller
{
    private readonly IKnowledgeAreaService _knowledgeAreaService;
    private readonly IDepartmentService _departmentService;

    public KnowledgeAreasController(
        IKnowledgeAreaService knowledgeAreaService,
        IDepartmentService departmentService)
    {
        _knowledgeAreaService = knowledgeAreaService;
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _knowledgeAreaService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _knowledgeAreaService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadDepartmentsAsync();
        return View(new CreateKnowledgeAreaDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateKnowledgeAreaDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }

        try
        {
            await _knowledgeAreaService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Área de conocimiento creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _knowledgeAreaService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateKnowledgeAreaDto
        {
            Id = item.Id,
            Name = item.Name,
            DepartmentId = item.DepartmentId
        };

        await LoadDepartmentsAsync(dto.DepartmentId);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateKnowledgeAreaDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }

        try
        {
            var updated = await _knowledgeAreaService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Área de conocimiento actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDepartmentsAsync(dto.DepartmentId);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _knowledgeAreaService.GetByIdAsync(id);

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
        var deleted = await _knowledgeAreaService.SoftDeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Área de conocimiento eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadDepartmentsAsync(int? selectedDepartmentId = null)
    {
        var departments = await _departmentService.GetAllAsync();

        ViewBag.Departments = departments
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name,
                Selected = selectedDepartmentId.HasValue && x.Id == selectedDepartmentId.Value
            })
            .ToList();
    }
}