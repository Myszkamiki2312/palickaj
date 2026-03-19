using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ErpSupportDesk.Data;
using ErpSupportDesk.Models;
using ErpSupportDesk.Services;
using ErpSupportDesk.ViewModels;

namespace ErpSupportDesk.Controllers;

public sealed class TicketsController(AppDbContext db, TicketNumberGenerator ticketNumberGenerator) : Controller
{
    public async Task<IActionResult> Index(string search = "", string status = "", string priority = "", string module = "")
    {
        var tickets = await BuildTicketQuery(search, status, priority, module).ToListAsync();

        var model = new TicketIndexViewModel
        {
            Search = search,
            Status = status,
            Priority = priority,
            Module = module,
            Tickets = tickets,
            StatusOptions = SupportCatalog.TicketStatuses,
            PriorityOptions = SupportCatalog.TicketPriorities,
            ModuleOptions = SupportCatalog.Modules
        };

        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var ticket = await db.ServiceTickets
            .AsNoTracking()
            .Include(item => item.Client)
            .FirstOrDefaultAsync(item => item.Id == id);

        return ticket is null ? NotFound() : View(ticket);
    }

    public async Task<IActionResult> Create()
    {
        var model = new TicketFormViewModel
        {
            AssignedEngineer = SupportCatalog.Engineers[0],
            SourceChannel = SupportCatalog.SourceChannels[0],
            Module = SupportCatalog.Modules[0],
            AffectedVersion = "ERP XL 2024.1"
        };

        return View(await PopulateFormAsync(model));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TicketFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(await PopulateFormAsync(model));
        }

        var now = DateTime.Now;
        var ticket = new ServiceTicket
        {
            ClientId = model.ClientId,
            Number = await ticketNumberGenerator.GenerateAsync(),
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            Module = model.Module,
            Status = model.Status,
            Priority = model.Priority,
            AssignedEngineer = model.AssignedEngineer,
            PlannedHours = model.PlannedHours,
            SpentHours = model.SpentHours,
            SourceChannel = model.SourceChannel,
            AffectedVersion = model.AffectedVersion.Trim(),
            CreatedAt = now,
            UpdatedAt = now,
            DueAt = model.DueAt,
            RequiresDeployment = model.RequiresDeployment,
            IsBillable = model.IsBillable
        };

        db.ServiceTickets.Add(ticket);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ticket = await db.ServiceTickets.FindAsync(id);

        if (ticket is null)
        {
            return NotFound();
        }

        return View(await PopulateFormAsync(TicketFormViewModel.FromEntity(ticket)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, TicketFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(await PopulateFormAsync(model));
        }

        var ticket = await db.ServiceTickets.FindAsync(id);

        if (ticket is null)
        {
            return NotFound();
        }

        ticket.ClientId = model.ClientId;
        ticket.Title = model.Title.Trim();
        ticket.Description = model.Description.Trim();
        ticket.Module = model.Module;
        ticket.Status = model.Status;
        ticket.Priority = model.Priority;
        ticket.AssignedEngineer = model.AssignedEngineer;
        ticket.PlannedHours = model.PlannedHours;
        ticket.SpentHours = model.SpentHours;
        ticket.SourceChannel = model.SourceChannel;
        ticket.AffectedVersion = model.AffectedVersion.Trim();
        ticket.DueAt = model.DueAt;
        ticket.RequiresDeployment = model.RequiresDeployment;
        ticket.IsBillable = model.IsBillable;
        ticket.UpdatedAt = DateTime.Now;

        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = ticket.Id });
    }

    public async Task<FileResult> ExportCsv(string search = "", string status = "", string priority = "", string module = "")
    {
        var tickets = await BuildTicketQuery(search, status, priority, module).ToListAsync();
        var builder = new StringBuilder();

        builder.AppendLine("Number,Client,Title,Module,Priority,Status,AssignedEngineer,PlannedHours,SpentHours,DueAt");

        foreach (var ticket in tickets)
        {
            builder.AppendLine(string.Join(",",
                Escape(ticket.Number),
                Escape(ticket.Client?.Name ?? string.Empty),
                Escape(ticket.Title),
                Escape(ticket.Module),
                Escape(ticket.Priority),
                Escape(ticket.Status),
                Escape(ticket.AssignedEngineer),
                ticket.PlannedHours.ToString(CultureInfo.InvariantCulture),
                ticket.SpentHours.ToString(CultureInfo.InvariantCulture),
                ticket.DueAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture)));
        }

        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
        return File(bytes, "text/csv", $"tickets-{DateTime.Today:yyyyMMdd}.csv");
    }

    private IQueryable<ServiceTicket> BuildTicketQuery(string search, string status, string priority, string module)
    {
        var query = db.ServiceTickets
            .AsNoTracking()
            .Include(ticket => ticket.Client)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(ticket =>
                ticket.Number.Contains(term) ||
                ticket.Title.Contains(term) ||
                ticket.Client!.Name.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(ticket => ticket.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(priority))
        {
            query = query.Where(ticket => ticket.Priority == priority);
        }

        if (!string.IsNullOrWhiteSpace(module))
        {
            query = query.Where(ticket => ticket.Module == module);
        }

        return query
            .OrderBy(ticket => ticket.Status == "Zamkniete")
            .ThenBy(ticket => ticket.DueAt)
            .ThenBy(ticket => ticket.Priority);
    }

    private async Task<TicketFormViewModel> PopulateFormAsync(TicketFormViewModel model)
    {
        var clients = await db.Clients
            .AsNoTracking()
            .OrderBy(client => client.Name)
            .ToListAsync();

        model.ClientOptions = clients.Select(client => new SelectListItem(client.Name, client.Id.ToString(), client.Id == model.ClientId)).ToList();
        model.StatusOptions = SupportCatalog.TicketStatuses.Select(item => new SelectListItem(item, item, item == model.Status)).ToList();
        model.PriorityOptions = SupportCatalog.TicketPriorities.Select(item => new SelectListItem(item, item, item == model.Priority)).ToList();
        model.ModuleOptions = SupportCatalog.Modules.Select(item => new SelectListItem(item, item, item == model.Module)).ToList();
        model.EngineerOptions = SupportCatalog.Engineers.Select(item => new SelectListItem(item, item, item == model.AssignedEngineer)).ToList();
        model.SourceOptions = SupportCatalog.SourceChannels.Select(item => new SelectListItem(item, item, item == model.SourceChannel)).ToList();

        return model;
    }

    private static string Escape(string value)
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
