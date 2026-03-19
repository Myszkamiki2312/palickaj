using ErpSupportDesk.Models;

namespace ErpSupportDesk.ViewModels;

public sealed class TicketIndexViewModel
{
    public string Search { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Priority { get; init; } = string.Empty;

    public string Module { get; init; } = string.Empty;

    public IReadOnlyList<ServiceTicket> Tickets { get; init; } = [];

    public IReadOnlyList<string> StatusOptions { get; init; } = [];

    public IReadOnlyList<string> PriorityOptions { get; init; } = [];

    public IReadOnlyList<string> ModuleOptions { get; init; } = [];
}
