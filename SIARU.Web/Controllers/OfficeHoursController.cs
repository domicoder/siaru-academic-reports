using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SIARU.Application.Common;
using SIARU.Application.DTOs.OfficeHours;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff}")]
public class OfficeHoursController : Controller
{
    private readonly IOfficeHourManagementService _officeHourManagementService;
    private readonly IProfessorManagementService _professorManagementService;

    public OfficeHoursController(
        IOfficeHourManagementService officeHourManagementService,
        IProfessorManagementService professorManagementService)
    {
        _officeHourManagementService = officeHourManagementService;
        _professorManagementService = professorManagementService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _officeHourManagementService.GetAllAsync();
        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _officeHourManagementService.GetByIdAsync(id);

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
        return View(new CreateOfficeHourDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOfficeHourDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectorsAsync(dto.ProfessorId, dto.DayOfWeek);
            return View(dto);
        }

        try
        {
            await _officeHourManagementService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Horario de consulta creado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectorsAsync(dto.ProfessorId, dto.DayOfWeek);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _officeHourManagementService.GetByIdAsync(id);

        if (item is null)
        {
            return NotFound();
        }

        var dto = new UpdateOfficeHourDto
        {
            Id = item.Id,
            ProfessorId = item.ProfessorId,
            DayOfWeek = item.DayOfWeek,
            StartTime = item.StartTime,
            EndTime = item.EndTime
        };

        await LoadSelectorsAsync(dto.ProfessorId, dto.DayOfWeek);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateOfficeHourDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectorsAsync(dto.ProfessorId, dto.DayOfWeek);
            return View(dto);
        }

        try
        {
            var updated = await _officeHourManagementService.UpdateAsync(dto);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Horario de consulta actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectorsAsync(dto.ProfessorId, dto.DayOfWeek);
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _officeHourManagementService.GetByIdAsync(id);

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
        var deleted = await _officeHourManagementService.SoftDeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        TempData["SuccessMessage"] = "Horario de consulta eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSelectorsAsync(int? selectedProfessorId = null, string? selectedDayOfWeek = null)
    {
        var professors = await _professorManagementService.GetAllAsync();

        ViewBag.Professors = professors
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} - {x.Office} ({x.KnowledgeAreaName})",
                Selected = selectedProfessorId.HasValue && x.Id == selectedProfessorId.Value
            })
            .ToList();

        ViewBag.WeekDays = new List<SelectListItem>
        {
            new() { Value = "Monday", Text = "Lunes", Selected = selectedDayOfWeek == "Monday" || selectedDayOfWeek == "Lunes" },
            new() { Value = "Tuesday", Text = "Martes", Selected = selectedDayOfWeek == "Tuesday" || selectedDayOfWeek == "Martes" },
            new() { Value = "Wednesday", Text = "Miércoles", Selected = selectedDayOfWeek == "Wednesday" || selectedDayOfWeek == "Miércoles" || selectedDayOfWeek == "Miercoles" },
            new() { Value = "Thursday", Text = "Jueves", Selected = selectedDayOfWeek == "Thursday" || selectedDayOfWeek == "Jueves" },
            new() { Value = "Friday", Text = "Viernes", Selected = selectedDayOfWeek == "Friday" || selectedDayOfWeek == "Viernes" },
            new() { Value = "Saturday", Text = "Sábado", Selected = selectedDayOfWeek == "Saturday" || selectedDayOfWeek == "Sábado" || selectedDayOfWeek == "Sabado" },
            new() { Value = "Sunday", Text = "Domingo", Selected = selectedDayOfWeek == "Sunday" || selectedDayOfWeek == "Domingo" }
        };
    }
}