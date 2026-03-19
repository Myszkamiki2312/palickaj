using Microsoft.AspNetCore.Mvc;
using Palickaj.Services;

namespace Palickaj.Controllers;

public sealed class ReportsController(DashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await dashboardService.BuildReportAsync();
        return View(model);
    }
}
