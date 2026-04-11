using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIARU.Application.Common;

namespace SIARU.Web.Controllers;

[Authorize(Roles = $"{RoleNames.Administrator},{RoleNames.StudentServicesStaff},{RoleNames.ReadOnlyUser}")]
public class ReportsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}