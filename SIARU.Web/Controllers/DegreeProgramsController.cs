using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIARU.Application.Common;
using SIARU.Application.DTOs.DegreePrograms;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff}")]
public class DegreeProgramsController : Controller
{
    private readonly IDegreeProgramService _degreeProgramService;

    public DegreeProgramsController(IDegreeProgramService degreeProgramService)
    {
        _degreeProgramService = degreeProgramService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _degreeProgramService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _degreeProgramService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateDegreeProgramDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateDegreeProgramDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _degreeProgramService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Titulación creada correctamente.";
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
        var item = await _degreeProgramService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateDegreeProgramDto
        {
            Id = item.Id,
            Name = item.Name
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateDegreeProgramDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            var updated = await _degreeProgramService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Titulación actualizada correctamente.";
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
        var item = await _degreeProgramService.GetByIdAsync(id);

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
        var deleted = await _degreeProgramService.SoftDeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Titulación eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}