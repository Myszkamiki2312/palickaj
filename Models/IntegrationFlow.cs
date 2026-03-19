using System.ComponentModel.DataAnnotations;

namespace ErpSupportDesk.Models;

public sealed class IntegrationFlow
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public Client? Client { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string SourceSystem { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string TargetSystem { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = string.Empty;

    [Required]
    [StringLength(80)]
    public string Owner { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Criticality { get; set; } = string.Empty;

    [Range(0, 100)]
    public decimal SuccessRate { get; set; }

    [Range(0, 999)]
    public int ErrorCount { get; set; }

    public DateTime LastRunAt { get; set; }

    public DateTime NextRunAt { get; set; }
}
