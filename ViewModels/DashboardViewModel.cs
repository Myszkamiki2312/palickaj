namespace ErpSupportDesk.ViewModels;

public sealed class DashboardViewModel
{
    public IReadOnlyList<MetricCardViewModel> Metrics { get; init; } = [];

    public IReadOnlyList<ModuleLoadViewModel> ModuleWorkload { get; init; } = [];

    public IReadOnlyList<TicketSnapshotViewModel> PriorityQueue { get; init; } = [];

    public IReadOnlyList<IntegrationSnapshotViewModel> IntegrationHealth { get; init; } = [];

    public IReadOnlyList<DeadlineSnapshotViewModel> UpcomingDeadlines { get; init; } = [];

    public IReadOnlyList<ClientPulseViewModel> ClientPulse { get; init; } = [];
}

public sealed record MetricCardViewModel(string Title, string Value, string AccentClass, string Hint);

public sealed record ModuleLoadViewModel(string Module, int Tickets, decimal Progress);

public sealed record TicketSnapshotViewModel(
    int Id,
    string Number,
    string Title,
    string Client,
    string Module,
    string Priority,
    string Status,
    string AssignedEngineer,
    DateTime DueAt);

public sealed record IntegrationSnapshotViewModel(
    int Id,
    string Name,
    string Client,
    string Status,
    string Owner,
    decimal SuccessRate,
    int ErrorCount,
    DateTime NextRunAt);

public sealed record DeadlineSnapshotViewModel(
    string Number,
    string Title,
    string Client,
    DateTime DueAt,
    string Status);

public sealed record ClientPulseViewModel(
    int Id,
    string Name,
    string SupportPlan,
    int OpenTickets,
    int ActiveUsers,
    decimal IntegrationHealth,
    DateTime LastReviewAt);
