using Palickaj.Models;

namespace Palickaj.ViewModels;

public sealed class IntegrationsIndexViewModel
{
    public string Search { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Owner { get; init; } = string.Empty;

    public IReadOnlyList<string> StatusOptions { get; init; } = [];

    public IReadOnlyList<string> OwnerOptions { get; init; } = [];

    public IReadOnlyList<IntegrationFlow> Integrations { get; init; } = [];
}
