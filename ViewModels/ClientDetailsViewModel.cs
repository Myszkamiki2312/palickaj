using ErpSupportDesk.Models;

namespace ErpSupportDesk.ViewModels;

public sealed class ClientDetailsViewModel
{
    public required Client Client { get; init; }

    public IReadOnlyList<ServiceTicket> ActiveTickets { get; init; } = [];

    public IReadOnlyList<ServiceTicket> RecentTickets { get; init; } = [];

    public IReadOnlyList<IntegrationFlow> Integrations { get; init; } = [];
}
