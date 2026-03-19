namespace Palickaj.ViewModels;

public sealed class ClientsIndexViewModel
{
    public string Search { get; init; } = string.Empty;

    public string SupportPlan { get; init; } = string.Empty;

    public IReadOnlyList<string> SupportPlanOptions { get; init; } = [];

    public IReadOnlyList<ClientListItemViewModel> Clients { get; init; } = [];
}

public sealed record ClientListItemViewModel(
    int Id,
    string Name,
    string Industry,
    string ErpEnvironment,
    string SupportPlan,
    int OpenTickets,
    int ActiveUsers,
    decimal IntegrationHealth);
