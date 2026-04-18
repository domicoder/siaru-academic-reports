using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIARU.Application.Common;
using SIARU.Application.DTOs.Departments;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff}")]
public class DepartmentsController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _departmentService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _departmentService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateDepartmentDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDepartmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _departmentService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Departamento creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(dto.Name), ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _departmentService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDepartmentDto
        {
            Id = item.Id,
            Name = item.Name
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDepartmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            var updated = await _departmentService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Departamento actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(dto.Name), ex.Message);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _departmentService.GetByIdAsync(id);

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
        var deleted = await _departmentService.SoftDeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Departamento eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}