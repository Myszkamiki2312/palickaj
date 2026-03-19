using Microsoft.AspNetCore.Mvc;
using ErpSupportDesk.Services;

namespace ErpSupportDesk.Controllers;

public sealed class ReportsController(DashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await dashboardService.BuildReportAsync();
        return View(model);
    }
}
