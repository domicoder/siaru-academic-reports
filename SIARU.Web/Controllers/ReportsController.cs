using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIARU.Application.Common;
using SIARU.Application.Interfaces;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff},{RoleNames.ReadOnlyUser}")]
public class ReportsController : Controller
{
    private readonly ISubjectReportService _subjectReportService;
    private readonly IProfessorReportService _professorReportService;

    public ReportsController(
        ISubjectReportService subjectReportService,
        IProfessorReportService professorReportService)
    {
        _subjectReportService = subjectReportService;
        _professorReportService = professorReportService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Subject(string? term)
    {
        var results = await _subjectReportService.SearchAsync(term);
        ViewBag.SearchTerm = term;
        return View(results);
    }

    [HttpGet]
    public async Task<IActionResult> SubjectDetails(string code)
    {
        var report = await _subjectReportService.GetByCodeAsync(code);

        if (report is null)
        {
            return NotFound();
        }

        return View(report);
    }

    [HttpGet]
    public async Task<IActionResult> Professor(string? term)
    {
        var results = await _professorReportService.SearchAsync(term);
        ViewBag.SearchTerm = term;
        return View(results);
    }

    [HttpGet]
    public async Task<IActionResult> ProfessorDetails(int id)
    {
        var report = await _professorReportService.GetByIdAsync(id);

        if (report is null)
        {
            return NotFound();
        }

        return View(report);
    }
}