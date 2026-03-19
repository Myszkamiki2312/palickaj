using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Palickaj.Data;
using Palickaj.Models;
using Palickaj.ViewModels;

namespace Palickaj.Controllers;

public sealed class IntegrationsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string search = "", string status = "", string owner = "")
    {
        var query = db.IntegrationFlows
            .AsNoTracking()
            .Include(integration => integration.Client)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(integration =>
                integration.Name.Contains(term) ||
                integration.SourceSystem.Contains(term) ||
                integration.TargetSystem.Contains(term) ||
                integration.Client!.Name.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(integration => integration.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(owner))
        {
            query = query.Where(integration => integration.Owner == owner);
        }

        var integrations = await query
            .OrderBy(integration => integration.Status == "Wymaga reakcji" ? 0 : integration.Status == "Monitorowana" ? 1 : 2)
            .ThenByDescending(integration => integration.ErrorCount)
            .ToListAsync();

        var model = new IntegrationsIndexViewModel
        {
            Search = search,
            Status = status,
            Owner = owner,
            StatusOptions = SupportCatalog.IntegrationStatuses,
            OwnerOptions = SupportCatalog.Engineers,
            Integrations = integrations
        };

        return View(model);
    }
}
