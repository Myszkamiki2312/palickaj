namespace Palickaj.ViewModels;

public sealed class ReportViewModel
{
    public IReadOnlyList<MetricCardViewModel> Summary { get; init; } = [];

    public IReadOnlyList<ModuleLoadViewModel> ModuleHealth { get; init; } = [];

    public IReadOnlyList<TicketSnapshotViewModel> OverdueTickets { get; init; } = [];

    public IReadOnlyList<IntegrationSnapshotViewModel> RiskyIntegrations { get; init; } = [];

    public IReadOnlyList<string> Recommendations { get; init; } = [];
}
