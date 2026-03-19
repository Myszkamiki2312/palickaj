using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Palickaj.Models;
using Palickaj.Services;

namespace Palickaj.Controllers;

public sealed class HomeController(DashboardService dashboardService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = await dashboardService.BuildDashboardAsync();
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
