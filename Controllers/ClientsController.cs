using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ErpSupportDesk.Data;
using ErpSupportDesk.Models;
using ErpSupportDesk.ViewModels;

namespace ErpSupportDesk.Controllers;

public sealed class ClientsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(string search = "", string supportPlan = "")
    {
        var clients = await db.Clients
            .AsNoTracking()
            .Include(client => client.Tickets)
            .Include(client => client.Integrations)
            .OrderBy(client => client.Name)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            clients = clients
                .Where(client =>
                    client.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.Industry.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    client.DatabaseName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(supportPlan))
        {
            clients = clients.Where(client => client.SupportPlan == supportPlan).ToList();
        }

        var model = new ClientsIndexViewModel
        {
            Search = search,
            SupportPlan = supportPlan,
            SupportPlanOptions = SupportCatalog.SupportPlans,
            Clients = clients
                .Select(client => new ClientListItemViewModel(
                    client.Id,
                    client.Name,
                    client.Industry,
                    client.ErpEnvironment,
                    client.SupportPlan,
                    client.Tickets.Count(ticket => ticket.Status != "Zamkniete"),
                    client.ActiveUsers,
                    client.Integrations.Count == 0 ? 100 : Math.Round(client.Integrations.Average(integration => integration.SuccessRate), 1)))
                .ToList()
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var client = await db.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (client is null)
        {
            return NotFound();
        }

        var activeTickets = await db.ServiceTickets
            .AsNoTracking()
            .Where(ticket => ticket.ClientId == id && ticket.Status != "Zamkniete")
            .OrderBy(ticket => ticket.DueAt)
            .ToListAsync();

        var recentTickets = await db.ServiceTickets
            .AsNoTracking()
            .Where(ticket => ticket.ClientId == id)
            .OrderByDescending(ticket => ticket.UpdatedAt)
            .Take(5)
            .ToListAsync();

        var integrations = await db.IntegrationFlows
            .AsNoTracking()
            .Where(integration => integration.ClientId == id)
            .OrderBy(integration => integration.Status)
            .ThenBy(integration => integration.Name)
            .ToListAsync();

        return View(new ClientDetailsViewModel
        {
            Client = client,
            ActiveTickets = activeTickets,
            RecentTickets = recentTickets,
            Integrations = integrations
        });
    }
}
