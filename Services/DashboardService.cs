using Microsoft.EntityFrameworkCore;
using Palickaj.Data;
using Palickaj.Models;
using Palickaj.ViewModels;

namespace Palickaj.Services;

public sealed class DashboardService(AppDbContext db)
{
    public async Task<DashboardViewModel> BuildDashboardAsync()
    {
        var today = DateTime.Today;
        var tickets = await db.ServiceTickets
            .AsNoTracking()
            .Include(ticket => ticket.Client)
            .OrderBy(ticket => ticket.DueAt)
            .ToListAsync();

        var integrations = await db.IntegrationFlows
            .AsNoTracking()
            .Include(integration => integration.Client)
            .OrderBy(integration => integration.NextRunAt)
            .ToListAsync();

        var clients = await db.Clients
            .AsNoTracking()
            .OrderBy(client => client.Name)
            .ToListAsync();

        var openTickets = tickets.Where(ticket => ticket.Status != "Zamkniete").ToList();
        var overdueTickets = openTickets.Where(ticket => ticket.DueAt < DateTime.Now).ToList();
        var criticalTickets = openTickets.Where(ticket => ticket.Priority == "Krytyczny").ToList();
        var dueToday = openTickets.Where(ticket => ticket.DueAt.Date == today).ToList();
        var monthlyBillableHours = tickets
            .Where(ticket => ticket.IsBillable && ticket.CreatedAt.Month == today.Month && ticket.CreatedAt.Year == today.Year)
            .Sum(ticket => ticket.SpentHours);
        var averageSla = openTickets.Count == 0
            ? 100
            : Math.Round(openTickets.Count(ticket => ticket.DueAt >= DateTime.Now) * 100m / openTickets.Count, 1);

        return new DashboardViewModel
        {
            Metrics =
            [
                new MetricCardViewModel("Otwarte zgloszenia", openTickets.Count.ToString(), "accent-red", $"{criticalTickets.Count} krytyczne"),
                new MetricCardViewModel("Dzisiaj do domkniecia", dueToday.Count.ToString(), "accent-gold", $"{overdueTickets.Count} po terminie"),
                new MetricCardViewModel("Godziny billable", $"{monthlyBillableHours:0.#}h", "accent-blue", "biezacy miesiac"),
                new MetricCardViewModel("SLA", $"{averageSla:0.#}%", "accent-green", "udzial zgloszen w terminie")
            ],
            ModuleWorkload = openTickets
                .GroupBy(ticket => ticket.Module)
                .Select(group => new ModuleLoadViewModel(
                    group.Key,
                    group.Count(),
                    Math.Min(100, group.Sum(ticket => ticket.PlannedHours) * 5)))
                .OrderByDescending(item => item.Tickets)
                .ToList(),
            PriorityQueue = openTickets
                .OrderBy(ticket => ticket.Priority == "Krytyczny" ? 0 : ticket.Priority == "Wysoki" ? 1 : 2)
                .ThenBy(ticket => ticket.DueAt)
                .Take(6)
                .Select(ticket => new TicketSnapshotViewModel(
                    ticket.Id,
                    ticket.Number,
                    ticket.Title,
                    ticket.Client?.Name ?? "Brak klienta",
                    ticket.Module,
                    ticket.Priority,
                    ticket.Status,
                    ticket.AssignedEngineer,
                    ticket.DueAt))
                .ToList(),
            IntegrationHealth = integrations
                .OrderBy(integration => integration.Status == "Wymaga reakcji" ? 0 : integration.Status == "Monitorowana" ? 1 : 2)
                .ThenBy(integration => integration.ErrorCount)
                .Take(6)
                .Select(integration => new IntegrationSnapshotViewModel(
                    integration.Id,
                    integration.Name,
                    integration.Client?.Name ?? "Brak klienta",
                    integration.Status,
                    integration.Owner,
                    integration.SuccessRate,
                    integration.ErrorCount,
                    integration.NextRunAt))
                .ToList(),
            UpcomingDeadlines = openTickets
                .OrderBy(ticket => ticket.DueAt)
                .Take(6)
                .Select(ticket => new DeadlineSnapshotViewModel(
                    ticket.Number,
                    ticket.Title,
                    ticket.Client?.Name ?? "Brak klienta",
                    ticket.DueAt,
                    ticket.Status))
                .ToList(),
            ClientPulse = clients
                .Select(client =>
                {
                    var clientTickets = openTickets.Where(ticket => ticket.ClientId == client.Id).ToList();
                    var clientIntegrations = integrations.Where(integration => integration.ClientId == client.Id).ToList();
                    var health = clientIntegrations.Count == 0
                        ? 100
                        : Math.Round(clientIntegrations.Average(integration => integration.SuccessRate), 1);

                    return new ClientPulseViewModel(
                        client.Id,
                        client.Name,
                        client.SupportPlan,
                        clientTickets.Count,
                        client.ActiveUsers,
                        health,
                        client.LastReviewAt);
                })
                .OrderByDescending(item => item.OpenTickets)
                .ThenBy(item => item.Name)
                .Take(6)
                .ToList()
        };
    }

    public async Task<ReportViewModel> BuildReportAsync()
    {
        var today = DateTime.Today;
        var tickets = await db.ServiceTickets
            .AsNoTracking()
            .Include(ticket => ticket.Client)
            .OrderBy(ticket => ticket.DueAt)
            .ToListAsync();

        var integrations = await db.IntegrationFlows
            .AsNoTracking()
            .Include(integration => integration.Client)
            .OrderBy(integration => integration.NextRunAt)
            .ToListAsync();

        var openTickets = tickets.Where(ticket => ticket.Status != "Zamkniete").ToList();
        var overdueTickets = openTickets.Where(ticket => ticket.DueAt < DateTime.Now).ToList();
        var riskyIntegrations = integrations.Where(integration => integration.Status != "Stabilna").ToList();
        var deploymentQueue = openTickets.Count(ticket => ticket.RequiresDeployment);
        var closedThisMonth = tickets.Count(ticket =>
            ticket.Status == "Zamkniete" &&
            ticket.UpdatedAt.Month == today.Month &&
            ticket.UpdatedAt.Year == today.Year);
        var averageCompletion = tickets.Count == 0
            ? 0
            : Math.Round(tickets.Where(ticket => ticket.PlannedHours > 0).Average(ticket => Math.Min(100, ticket.SpentHours / ticket.PlannedHours * 100)), 1);

        var recommendations = new List<string>();

        if (overdueTickets.Count > 0)
        {
            recommendations.Add($"Priorytetem jest domkniecie {overdueTickets.Count} zgloszen po terminie, szczegolnie w moduach produkcyjnych i logistycznych.");
        }

        if (riskyIntegrations.Any(integration => integration.Status == "Wymaga reakcji"))
        {
            recommendations.Add("Warto przygotowac prosty playbook do obslugi awarii integracji i raport dzienny z ostatnich bledow.");
        }

        if (deploymentQueue > 2)
        {
            recommendations.Add("Kolejka wdrozen jest juz zauwazalna, dlatego dobrze zaplanowac jedno okno serwisowe i zgrupowac deploymenty.");
        }

        if (closedThisMonth < 3)
        {
            recommendations.Add("Tempo zamykania zgloszen jest niskie, wiec przydalby sie szybki przeglad zadan oczekujacych na dane od klienta.");
        }

        if (recommendations.Count == 0)
        {
            recommendations.Add("Portfel zadan wyglada stabilnie i nie wymaga pilnej eskalacji.");
        }

        return new ReportViewModel
        {
            Summary =
            [
                new MetricCardViewModel("Otwarte zgloszenia", openTickets.Count.ToString(), "accent-red", "aktywny backlog"),
                new MetricCardViewModel("Po terminie", overdueTickets.Count.ToString(), "accent-gold", "potrzebna reakcja"),
                new MetricCardViewModel("Integracje do uwagi", riskyIntegrations.Count.ToString(), "accent-blue", "monitorowane lub zagrozone"),
                new MetricCardViewModel("Sredni postep", $"{averageCompletion:0.#}%", "accent-green", "wzgledem planowanych godzin")
            ],
            ModuleHealth = openTickets
                .GroupBy(ticket => ticket.Module)
                .Select(group => new ModuleLoadViewModel(
                    group.Key,
                    group.Count(),
                    Math.Min(100, group.Average(ticket => ticket.PlannedHours == 0 ? 0 : ticket.SpentHours / ticket.PlannedHours * 100))))
                .OrderByDescending(item => item.Tickets)
                .ToList(),
            OverdueTickets = overdueTickets
                .OrderBy(ticket => ticket.DueAt)
                .Select(ticket => new TicketSnapshotViewModel(
                    ticket.Id,
                    ticket.Number,
                    ticket.Title,
                    ticket.Client?.Name ?? "Brak klienta",
                    ticket.Module,
                    ticket.Priority,
                    ticket.Status,
                    ticket.AssignedEngineer,
                    ticket.DueAt))
                .ToList(),
            RiskyIntegrations = riskyIntegrations
                .OrderBy(integration => integration.Status == "Wymaga reakcji" ? 0 : 1)
                .ThenByDescending(integration => integration.ErrorCount)
                .Select(integration => new IntegrationSnapshotViewModel(
                    integration.Id,
                    integration.Name,
                    integration.Client?.Name ?? "Brak klienta",
                    integration.Status,
                    integration.Owner,
                    integration.SuccessRate,
                    integration.ErrorCount,
                    integration.NextRunAt))
                .ToList(),
            Recommendations = recommendations
        };
    }
}
